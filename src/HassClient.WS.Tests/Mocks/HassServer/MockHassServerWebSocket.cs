using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class MockHassServerWebSocket : MockServerWebSocket
    {
        public MockHassServerRequestContext RequestContext { get; }

        public CalVer HAVersion => CalVer.Create("2022.1.0");

        public ConnectionParameters ConnectionParameters { get; private set; }

        public bool IgnoreAuthenticationMessages { get; set; } = false;

        public TimeSpan ResponseSimulatedDelay { get; set; } = TimeSpan.Zero;

        public MockHassServerWebSocket()
            : base()
        {
            this.ConnectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(
                $"http://{this.ServerUri.Host}:{this.ServerUri.Port}",
                this.GenerateRandomToken());
            this.RequestContext = new MockHassServerRequestContext();
        }

        public Task<bool> RaiseStateChangedEventAsync(string entityId)
        {
            var data = MockHassModelFactory.StateChangedEventFaker
                                           .GenerateWithEntityId(entityId);

            var eventResult = new HassEvent()
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
            var eventSubscriptionsProcessor = this.RequestContext.GetCommandProcessor<EventSubscriptionsProcessor>();
            if (eventSubscriptionsProcessor.TryGetSubscribers(eventType, out var subscribers))
            {
                foreach (var id in subscribers)
                {
                    await this.RequestContext.SendMessageAsync(new IncomingEventMessage() { Event = eventResultObject, Id = id }, default);
                }

                return true;
            }

            return false;
        }

        private string GenerateRandomToken() => Guid.NewGuid().ToString("N");

        protected override async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            this.RequestContext.Initialize(webSocket);
            await this.RequestContext.SendMessageAsync(new AuthenticationRequiredMessage() { HAVersion = this.HAVersion.ToString() }, cancellationToken);

            try
            {
                while (true)
                {
                    if (this.RequestContext.IsAuthenticating)
                    {
                        var receivedMessage = await this.RequestContext.ReceiveMessageAsync<BaseMessage>(cancellationToken);
                        var shouldProcess = receivedMessage != null;
                        if (!shouldProcess)
                        {
                            continue;
                        }

                        await Task.Delay(this.ResponseSimulatedDelay);

                        if (!this.IgnoreAuthenticationMessages &&
                            receivedMessage is AuthenticationMessage authMessage)
                        {
                            if (authMessage.AccessToken == this.ConnectionParameters.AccessToken)
                            {
                                await this.RequestContext.SendMessageAsync(new AuthenticationOkMessage() { HAVersion = this.HAVersion.ToString() }, cancellationToken);
                                this.RequestContext.IsAuthenticating = false;
                            }
                            else
                            {
                                await this.RequestContext.SendMessageAsync(new AuthenticationInvalidMessage(), cancellationToken);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var receivedMessage = await this.RequestContext.ReceiveMessageAsync<BaseOutgoingMessage>(cancellationToken);
                        var shouldProcess = receivedMessage != null;
                        if (!shouldProcess)
                        {
                            continue;
                        }

                        var receivedMessageId = receivedMessage.Id;

                        await Task.Delay(this.ResponseSimulatedDelay);


                        BaseIdentifiableMessage response;
                        if (this.RequestContext.LastReceivedID >= receivedMessageId)
                        {
                            response = new ResultMessage() { Error = new ErrorInfo(ErrorCodes.IdReuse) };
                        }
                        else
                        {
                            this.RequestContext.LastReceivedID = receivedMessageId;

                            if (receivedMessage is PingMessage)
                            {
                                response = new PongMessage();
                            }
                            else if (!this.RequestContext.TryProcessMessage(receivedMessage, out response))
                            {
                                response = new ResultMessage() { Error = new ErrorInfo(ErrorCodes.UnknownCommand) };
                            }
                        }

                        response.Id = receivedMessageId;
                        await this.RequestContext.SendMessageAsync(response, cancellationToken);
                    }
                }
            }
            catch
            {
                Trace.WriteLine("A problem occurred while attending client. Closing connection.");
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, default);
            }
        }
    }
}
