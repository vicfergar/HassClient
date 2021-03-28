using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerWebSocket : MockServerWebSocket
    {
        private readonly MockHassDB hassDB = new MockHassDB();

        private MockHassServerRequestContext activeRequestContext;

        public Version HAVersion => Version.Parse("0.117.1");

        public ConnectionParameters ConnectionParameters { get; private set; }

        public bool IgnoreAuthenticationMessages { get; set; } = false;

        public TimeSpan ResponseSimulatedDelay { get; set; } = TimeSpan.Zero;

        public MockHassServerWebSocket()
            : base()
        {
            this.ConnectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(
                $"http://{this.ServerUri.Host}:{this.ServerUri.Port}",
                this.GenerateRandomToken());
        }

        public Task<bool> RaiseStateChangedEventAsync(string entityId)
        {
            var data = MockHassModelFactory.StateChangedEventFaker
                                           .GenerateWithEntityId(entityId);

            var eventResult = new EventResultInfo()
            {
                EventType = KnownEventTypes.StateChanged.ToEventTypeString(),
                Origin = "mock_server",
                TimeFired = DateTimeOffset.Now,
                Data = new JRaw(HassSerializer.SerializeObject(data)),
                Context = data.OldState.Context
            };

            var eventResultObject = new JRaw(HassSerializer.SerializeObject(eventResult));
            return this.RaiseEventAsync(KnownEventTypes.StateChanged, eventResultObject);
        }

        public async Task<bool> RaiseEventAsync(KnownEventTypes eventType, JRaw eventResultObject)
        {
            var context = this.activeRequestContext;
            if (context.EventSubscriptionsProcessor.TryGetSubscribers(eventType, out var subscribers))
            {
                foreach (var id in subscribers)
                {
                    await context.SendMessageAsync(new EventResultMessage() { Event = eventResultObject, Id = id }, default);
                }

                return true;
            }

            return false;
        }

        private string GenerateRandomToken() => Guid.NewGuid().ToString("N");

        protected override async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token)
        {
            var context = new MockHassServerRequestContext(this.hassDB, webSocket);

            await context.SendMessageAsync(new AuthenticationRequiredMessage() { HAVersion = this.HAVersion }, token);

            while (true)
            {
                if (context.IsAuthenticating)
                {
                    var incomingMessage = await context.ReceiveMessageAsync<BaseMessage>(token);
                    await Task.Delay(this.ResponseSimulatedDelay);

                    if (!this.IgnoreAuthenticationMessages &&
                        incomingMessage is AuthenticationMessage authMessage)
                    {
                        if (authMessage.AccessToken == this.ConnectionParameters.AccessToken)
                        {
                            await context.SendMessageAsync(new AuthenticationOkMessage() { HAVersion = this.HAVersion }, token);
                            context.IsAuthenticating = false;
                            this.activeRequestContext = context;
                        }
                        else
                        {
                            await context.SendMessageAsync(new AuthenticationInvalidMessage(), token);
                            break;
                        }
                    }
                }
                else
                {
                    var receivedMessage = await context.ReceiveMessageAsync<BaseOutgoingMessage>(token);
                    var receivedMessageId = receivedMessage.Id;

                    await Task.Delay(this.ResponseSimulatedDelay);

                    BaseIdentifiableMessage response;
                    if (context.LastReceivedID >= receivedMessageId)
                    {
                        response = new ResultMessage() { Error = new ErrorInfo(ErrorCodes.IdReuse) };
                    }
                    else
                    {
                        context.LastReceivedID = receivedMessageId;

                        if (receivedMessage is PingMessage)
                        {
                            response = new PongMessage();
                        }
                        else if (!context.TryProccesMessage(receivedMessage, out response))
                        {
                            response = new ResultMessage() { Error = new ErrorInfo(ErrorCodes.UnknownCommand) };
                        }
                    }

                    response.Id = receivedMessageId;
                    await context.SendMessageAsync(response, token);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, default);
        }
    }
}
