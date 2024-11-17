﻿using HassClient.Helpers;
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
        private MockHassServerRequestContext activeRequestContext;

        public CalVer HAVersion => CalVer.Create("2022.1.0");

        public ConnectionParameters ConnectionParameters { get; private set; }

        public bool IgnoreAuthenticationMessages { get; set; } = false;

        public TimeSpan ResponseSimulatedDelay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// A function that can be used to intercept and modify messages before they are processed.
        /// If the function returns true, the message is processed normally. If it returns false, the message is ignored.
        /// </summary>
        public Func<BaseOutgoingMessage, bool> OnMessageReceived { get; set; } = null;

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
            var context = this.activeRequestContext;
            if (context.EventSubscriptionsProcessor.TryGetSubscribers(eventType, out var subscribers))
            {
                foreach (var id in subscribers)
                {
                    await context.SendMessageAsync(new IncomingEventMessage() { Event = eventResultObject, Id = id }, default);
                }

                return true;
            }

            return false;
        }

        private string GenerateRandomToken() => Guid.NewGuid().ToString("N");

        protected override async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var context = new MockHassServerRequestContext(webSocket);

            await context.SendMessageAsync(new AuthenticationRequiredMessage() { HAVersion = this.HAVersion.ToString() }, cancellationToken);

            try
            {
                while (true)
                {
                    if (context.IsAuthenticating)
                    {
                        var incomingMessage = await context.ReceiveMessageAsync<BaseMessage>(cancellationToken);
                        await Task.Delay(this.ResponseSimulatedDelay);

                        if (!this.IgnoreAuthenticationMessages &&
                            incomingMessage is AuthenticationMessage authMessage)
                        {
                            if (authMessage.AccessToken == this.ConnectionParameters.AccessToken)
                            {
                                await context.SendMessageAsync(new AuthenticationOkMessage() { HAVersion = this.HAVersion.ToString() }, cancellationToken);
                                context.IsAuthenticating = false;
                                this.activeRequestContext = context;
                            }
                            else
                            {
                                await context.SendMessageAsync(new AuthenticationInvalidMessage(), cancellationToken);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var receivedMessage = await context.ReceiveMessageAsync<BaseOutgoingMessage>(cancellationToken);
                        var receivedMessageId = receivedMessage.Id;

                        await Task.Delay(this.ResponseSimulatedDelay);

                        var shouldProcess = this.OnMessageReceived?.Invoke(receivedMessage) ?? true;
                        if (!shouldProcess)
                        {
                            continue;
                        }

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
                            else if (!context.TryProcessMessage(receivedMessage, out response))
                            {
                                response = new ResultMessage() { Error = new ErrorInfo(ErrorCodes.UnknownCommand) };
                            }
                        }

                        response.Id = receivedMessageId;
                        await context.SendMessageAsync(response, cancellationToken);
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
