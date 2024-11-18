using HassClient.Serialization;
using HassClient.WS.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerRequestContext
    {
        private const int INCOMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        private readonly List<BaseCommandProcessor> commandProcessors;

        private readonly ArraySegment<byte> receivingBuffer;

        public bool IsAuthenticating { get; set; }
        public uint LastReceivedID { get; set; }

        /// <summary>
        /// Intercepts incoming messages before they are processed.
        /// If the interceptor returns <c>true</c>, the message is processed normally; otherwise, it is skipped.
        /// </summary>
        public Func<BaseMessage, Task<bool>> IncomingMessageInterceptor { get; set; }

        /// <summary>
        /// Intercepts outgoing messages before they are sent.
        /// The interceptor can modify the message or return a different message, or even null to block the message from being sent.
        /// </summary>
        public Func<BaseMessage, Task<BaseMessage>> OutgoingMessageInterceptor { get; set; }

        private WebSocket webSocket;

        public MockHassServerRequestContext()
        {
            this.receivingBuffer = new ArraySegment<byte>(new byte[INCOMING_BUFFER_SIZE]);
            this.commandProcessors = new List<BaseCommandProcessor>()
            {
                new EventSubscriptionsProcessor(),
                new PingCommandProcessor(),
                new CallServiceCommandProcessor(),
                new RenderTemplateCommandProcessor(),
            };
        }

        public void Initialize(WebSocket webSocket)
        {
            this.webSocket = webSocket;
            this.IsAuthenticating = true;
            this.LastReceivedID = 0;
        }

        public TProcessor GetCommandProcessor<TProcessor>()
            where TProcessor : BaseCommandProcessor
        {
            return this.commandProcessors.OfType<TProcessor>().FirstOrDefault();
        }

        public bool TryProcessMessage(BaseIdentifiableMessage receivedCommand, out BaseIdentifiableMessage result)
        {
            var processor = this.commandProcessors.FirstOrDefault(x => x.CanProcess(receivedCommand));
            if (processor == null)
            {
                Trace.WriteLine($"[MockHassServer] No Command processor found for received message '{receivedCommand.Type}'");
                result = null;
                return false;
            }

            result = processor.ProcessCommand(this, receivedCommand);
            return true;
        }

        public async Task<TMessage> ReceiveMessageAsync<TMessage>(CancellationToken cancellationToken)
            where TMessage : BaseMessage
        {
            var receivedString = new StringBuilder();
            WebSocketReceiveResult rcvResult;
            do
            {
                rcvResult = await this.webSocket.ReceiveAsync(this.receivingBuffer, cancellationToken);
                byte[] msgBytes = this.receivingBuffer.Skip(this.receivingBuffer.Offset).Take(rcvResult.Count).ToArray();
                receivedString.Append(Encoding.UTF8.GetString(msgBytes));
            }
            while (!rcvResult.EndOfMessage);

            var rcvMsg = receivedString.ToString();
            var message = HassSerializer.DeserializeObject<TMessage>(rcvMsg);
            var interceptorTask = this.IncomingMessageInterceptor?.Invoke(message);
            if (interceptorTask != null)
            {
                var shouldProcess = await interceptorTask;
                if (!shouldProcess)
                {
                    return null;
                }
            }
            return message;
        }

        public async Task SendMessageAsync(BaseMessage message, CancellationToken cancellationToken)
        {
            var interceptorTask = this.OutgoingMessageInterceptor?.Invoke(message);
            if (interceptorTask != null)
            {
                message = await interceptorTask;
                if (message == null)
                {
                    return;
                }
            }

            var sendMsg = HassSerializer.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(sendMsg);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await this.webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken);
        }
    }
}
