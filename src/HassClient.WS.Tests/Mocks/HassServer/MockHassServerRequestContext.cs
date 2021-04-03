using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerRequestContext
    {
        private const int INCONMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        private readonly List<BaseCommandProcessor> commandProcessors;

        private readonly ArraySegment<byte> receivingBuffer;

        public readonly MockHassDB HassDB;

        public readonly EventSubscriptionsProcessor EventSubscriptionsProcessor;

        public bool IsAuthenticating { get; set; }
        public uint LastReceivedID { get; set; }

        public WebSocket WebSocket { get; private set; }

        public MockHassServerRequestContext(MockHassDB hassDB, WebSocket webSocket)
           : base()
        {
            this.IsAuthenticating = true;
            this.LastReceivedID = 0;
            this.HassDB = hassDB;
            this.WebSocket = webSocket;
            this.receivingBuffer = new ArraySegment<byte>(new byte[INCONMING_BUFFER_SIZE]);
            this.EventSubscriptionsProcessor = new EventSubscriptionsProcessor();
            this.commandProcessors = new List<BaseCommandProcessor>()
            {
                this.EventSubscriptionsProcessor,
                new PingCommandProcessor(),
                new GetConfigurationCommandProcessor(),
                new EntitySourceCommandProcessor(),
                new PanelsCommandProcessor(),
                new RenderTemplateCommandProcessor(),
                new SearchCommandProcessor(),
                new CallServiceCommandProcessor(),
                new GetServicesCommandProcessor(),
                new GetStatesCommandProcessor(),
                new RegistryEntryCollectionCommandProcessor<AreaRegistryMessagesFactory, Area>(),
                new DeviceStorageCollectionCommandProcessor(),
                new UserStorageCollectionCommandProcessor(),
                new EntityRegistryStorageCollectionCommandProcessor(),
                new StorageCollectionCommandProcessor<InputBoolean>(),
                new StorageCollectionCommandProcessor<Zone>(),
            };
        }

        public bool TryProccesMessage(BaseIdentifiableMessage receivedCommand, out BaseIdentifiableMessage result)
        {
            var processor = this.commandProcessors.FirstOrDefault(x => x.CanProcess(receivedCommand));
            if (processor == null)
            {
                Trace.WriteLine($"[MockHassServer] No Command processor found for received message '{receivedCommand.Type}'");
                result = null;
                return false;
            }

            result = processor.ProccessCommand(this, receivedCommand);
            return true;
        }

        public async Task<TMessage> ReceiveMessageAsync<TMessage>(CancellationToken cancellationToken)
            where TMessage : BaseMessage
        {
            var receivedString = new StringBuilder();
            WebSocketReceiveResult rcvResult;
            do
            {
                rcvResult = await this.WebSocket.ReceiveAsync(this.receivingBuffer, cancellationToken);
                byte[] msgBytes = this.receivingBuffer.Skip(this.receivingBuffer.Offset).Take(rcvResult.Count).ToArray();
                receivedString.Append(Encoding.UTF8.GetString(msgBytes));
            }
            while (!rcvResult.EndOfMessage);

            var rcvMsg = receivedString.ToString();
            return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
        }

        public async Task SendMessageAsync(BaseMessage message, CancellationToken cancellationToken)
        {
            var sendMsg = HassSerializer.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(sendMsg);
            var sendBuffer = new ArraySegment<byte>(sendBytes);
            await this.WebSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken);
        }
    }
}
