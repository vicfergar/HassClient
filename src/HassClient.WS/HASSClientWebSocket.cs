using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an abstraction layer over <see cref="ClientWebSocket"/> used by
    /// <see cref="HassWSApi"/> to send commands and subscribe for events.
    /// </summary>
    public class HassClientWebSocket : IDisposable
    {
        private const string TAG = "[" + nameof(HassClientWebSocket) + "]";

        private const int INCONMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        private readonly SemaphoreSlim sendingSemaphore = new SemaphoreSlim(1);

        private readonly Dictionary<string, SocketEventSubscription> socketEventSubscriptionIdByEventType = new Dictionary<string, SocketEventSubscription>();
        private readonly Dictionary<uint, Action<EventResultMessage>> socketEventCallbacksBySubsciptionId = new Dictionary<uint, Action<EventResultMessage>>();
        private readonly ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>> incomingMessageAwaitersById = new ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>>();

        private Channel<EventResultMessage> receivedEventsChannel;

        private ClientWebSocket socket;
        private CancellationTokenSource socketCTS;
        private uint lastSentID;
        private Task socketListenerTask;
        private Task eventListenerTask;
        private ConnectionStates connectionState;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDiposed { get; private set; }

        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        public ConnectionStates ConnectionState
        {
            get => this.connectionState;
            private set
            {
                if (this.connectionState != value)
                {
                    this.connectionState = value;
                    this.ConnectionStateChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Gets the connected Home Assistant instance version.
        /// </summary>
        public Version HAVersion { get; private set; }

        /// <summary>
        /// Gets the number of requests that are pending to be attended by the server.
        /// </summary>
        public int PendingRequestsCount => this.incomingMessageAwaitersById.Count;

        /// <summary>
        /// Gets the number of event handler subscriptions.
        /// </summary>
        public int SubscriptionsCount => (int)this.socketEventSubscriptionIdByEventType.Values.Sum(x => x.SubscriptionCount);

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        public event EventHandler<ConnectionStates> ConnectionStateChanged;

        static HassClientWebSocket()
        {
            var converters = HassSerializer.DefaultSettings.Converters;
            if (!converters.Any(x => x is MessagesConverter))
            {
                converters.Add(new MessagesConverter());
            }
        }

        /// <summary>
        /// Connects to a Home Assistant instance using the specified <see cref="Uri"/> and Access Token.
        /// (More info at <see href="https://developers.home-assistant.io/docs/api/websocket"/>).
        /// </summary>
        /// <param name="uri">
        /// An <see cref="Uri"/> representing the connection endpoint. (e.g. "ws://localhost:8123/api/websocket").
        /// </param>
        /// <param name="accessToken">
        /// The access token to be used during authentication phase.
        /// <para>
        /// You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser,
        /// and going to your profile http://IP_ADDRESS:8123/profile.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task ConnectAsync(Uri uri, string accessToken, CancellationToken cancellationToken = default)
        {
            this.CheckIsDiposed();

            if (this.ConnectionState != ConnectionStates.Disconnected)
            {
                throw new InvalidOperationException($"{nameof(HassClientWebSocket)} is not disconnected.");
            }

            this.ConnectionState = ConnectionStates.Connecting;

            this.socket = new ClientWebSocket();
            this.socketCTS = new CancellationTokenSource();
            this.lastSentID = 0;

            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.socketCTS.Token, cancellationToken);

            var rcvBytes = new byte[INCONMING_BUFFER_SIZE];
            var rcvBuffer = new ArraySegment<byte>(rcvBytes);
            try
            {
                await this.socket.ConnectAsync(uri, linkedCTS.Token);

                this.ConnectionState = ConnectionStates.Authenticating;

                var incomingMsg = await this.ReceiveMessage<BaseMessage>(rcvBuffer, linkedCTS.Token);
                if (incomingMsg is AuthenticationRequiredMessage authRequired)
                {
                    var authMsg = new AuthenticationMessage();
                    authMsg.AccessToken = accessToken;
                    await this.SendMessage(authMsg, linkedCTS.Token);

                    incomingMsg = await this.ReceiveMessage<BaseMessage>(rcvBuffer, linkedCTS.Token);
                    if (incomingMsg is AuthenticationInvalidMessage authenticationInvalid)
                    {
                        throw new InvalidOperationException($"{TAG} Invalid authentication: {authenticationInvalid.Message}");
                    }
                    else if (incomingMsg is AuthenticationOkMessage authenticationOk)
                    {
                        this.HAVersion = authenticationOk.HAVersion;
                        this.ConnectionState = ConnectionStates.Connected;
                        Trace.WriteLine($"{TAG} Authentication succeed. Client connected {nameof(this.HAVersion)}: {this.HAVersion}");
                    }
                }
                else
                {
                    throw new Exception("Unexpected message received during authentication.");
                }
            }
            catch (TaskCanceledException)
            {
                this.ClearSocketResources();
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"[{nameof(HassClientWebSocket)}] Connection failed: {ex}", ex);
            }

            this.receivedEventsChannel = Channel.CreateUnbounded<EventResultMessage>();

            this.socketListenerTask = Task.Factory.StartNew(
                async () =>
                {
                    while (this.socket.State.HasFlag(WebSocketState.Open))
                    {
                        var incomingMessage = await this.ReceiveMessage<BaseIncomingMessage>(rcvBuffer, this.socketCTS.Token);
                        if (incomingMessage is EventResultMessage eventResultMessage)
                        {
                            if (!this.receivedEventsChannel.Writer.TryWrite(eventResultMessage))
                            {
                                Debug.WriteLine($"{TAG} {nameof(this.receivedEventsChannel)} is full. One event message will discarded.");
                            }
                        }
                        else if (incomingMessage is PongMessage ||
                                 incomingMessage is ResultMessage)
                        {
                            Trace.WriteLine($"{TAG} Command message received {incomingMessage}");
                            if (this.incomingMessageAwaitersById.TryRemove(incomingMessage.Id, out var responseTCS))
                            {
                                responseTCS.SetResult(incomingMessage);
                            }
                            else
                            {
                                Debug.WriteLine($"{TAG} No awaiter found for incoming message {incomingMessage}. Message will be discarded.");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"{TAG} Unexpected message type received: {incomingMessage}");
                        }
                    }

                    // TODO: Handle socket connection lost and reconnection
                    Trace.WriteLine($"{TAG} Connection ended {this.socket.CloseStatus}");
                    this.ClearSocketResources();
                }, TaskCreationOptions.LongRunning);

            this.eventListenerTask = Task.Factory.StartNew(
                async () =>
                {
                    var channelReader = this.receivedEventsChannel.Reader;
                    while (await channelReader.WaitToReadAsync(this.socketCTS.Token))
                    {
                        while (channelReader.TryRead(out var incomingMessage))
                        {
                            if (this.socketEventCallbacksBySubsciptionId.TryGetValue(incomingMessage.Id, out var callback))
                            {
                                callback(incomingMessage);
                            }
                        }
                    }
                }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Close the Home Assistant connection as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            this.CheckIsDiposed();

            if (this.ConnectionState == ConnectionStates.Disconnected ||
                cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (this.socket.State == WebSocketState.Connecting)
            {
                this.socket.Abort();
            }
            else
            {
                await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by user", cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }

            this.ClearSocketResources();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!this.IsDiposed)
            {
                this.IsDiposed = true;
                this.ClearSocketResources();

                foreach (var item in this.socketEventSubscriptionIdByEventType.Values)
                {
                    item.ClearAllSubscriptions();
                }

                this.socketEventSubscriptionIdByEventType.Clear();
            }
        }

        private void CheckIsDiposed()
        {
            if (this.IsDiposed)
            {
                throw new ObjectDisposedException(nameof(HassClientWebSocket));
            }
        }

        private void ClearSocketResources()
        {
            if (this.ConnectionState != ConnectionStates.Disconnected)
            {
                this.ConnectionState = ConnectionStates.Disconnected;

                this.socketCTS?.Cancel();
                this.socket.Dispose();

                this.socketEventCallbacksBySubsciptionId.Clear();
                this.incomingMessageAwaitersById.Clear();
                this.receivedEventsChannel?.Writer.Complete();
            }
        }

        private async Task<TMessage> ReceiveMessage<TMessage>(ArraySegment<byte> buffer, CancellationToken cancellationToken)
                where TMessage : BaseMessage
        {
            var receivedString = new StringBuilder();
            WebSocketReceiveResult rcvResult;
            do
            {
                rcvResult = await this.socket.ReceiveAsync(buffer, cancellationToken);
                byte[] msgBytes = buffer.Skip(buffer.Offset).Take(rcvResult.Count).ToArray();
                receivedString.Append(Encoding.UTF8.GetString(msgBytes));
            }
            while (!rcvResult.EndOfMessage);

            try
            {
                var rcvMsg = receivedString.ToString();
                return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
            }
            catch (JsonException)
            {
                throw;
            }
        }

        private Task SendMessage(BaseMessage message, CancellationToken cancellationToken)
        {
            return this.SendMessage(message, null, cancellationToken);
        }

        private async Task SendMessage(BaseMessage message, TaskCompletionSource<BaseIncomingMessage> responseTCS, CancellationToken cancellationToken)
        {
            try
            {
                object toSerialize = message;
                await this.sendingSemaphore.WaitAsync(cancellationToken);
                if (message is BaseIdentifiableMessage identifiableMessage)
                {
                    identifiableMessage.Id = ++this.lastSentID;

                    if (message is SubscribeEventsMessage)
                    {
                        this.socketEventCallbacksBySubsciptionId.Add(identifiableMessage.Id, this.ProcessReceivedEventSubscriptionMessage);
                    }
                    else if (message is RenderTemplateMessage renderTemplateMessage)
                    {
                        this.socketEventCallbacksBySubsciptionId.Add(identifiableMessage.Id, renderTemplateMessage.ProcessEventReceivedMessage);
                    }
                    else if (message is RawCommandMessage rawCommand &&
                             rawCommand.MergedObject != null)
                    {
                        var mergedMessage = HassSerializer.CreateJObject(message);
                        var mergedObject = rawCommand.MergedObject as JObject ?? HassSerializer.CreateJObject(rawCommand.MergedObject);
                        mergedMessage.Merge(mergedObject);
                        toSerialize = mergedMessage;
                    }

                    if (responseTCS != null)
                    {
                        this.incomingMessageAwaitersById.TryAdd(identifiableMessage.Id, responseTCS);
                    }
                }

                var sendMsg = HassSerializer.SerializeObject(toSerialize);
                var sendBytes = Encoding.UTF8.GetBytes(sendMsg);
                var sendBuffer = new ArraySegment<byte>(sendBytes);
                await this.socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken);
                Trace.WriteLine($"{TAG} Message sent: {toSerialize}");
            }
            finally
            {
                this.sendingSemaphore.Release();
            }
        }

        private void ProcessReceivedEventSubscriptionMessage(EventResultMessage eventResultMessage)
        {
            var eventResultInfo = eventResultMessage.DeserializeEvent<EventResultInfo>();
            if (this.socketEventSubscriptionIdByEventType.TryGetValue(eventResultInfo.EventType, out var socketEventSubscription) &&
                socketEventSubscription.SubscriptionId == eventResultMessage.Id)
            {
                socketEventSubscription.Invoke(eventResultInfo);
            }

            if (this.socketEventSubscriptionIdByEventType.TryGetValue(Event.AnyEventFilter, out socketEventSubscription) &&
                socketEventSubscription.SubscriptionId == eventResultMessage.Id)
            {
                socketEventSubscription.Invoke(eventResultInfo);
            }
        }

        private async Task<ResultMessage> SendCommandAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            if (this.ConnectionState != ConnectionStates.Connected)
            {
                throw new InvalidOperationException($"Client not connected.");
            }

            try
            {
                var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.socketCTS.Token, cancellationToken);
                var responseTCS = new TaskCompletionSource<BaseIncomingMessage>(linkedCTS.Token);
                await this.SendMessage(commandMessage, responseTCS, linkedCTS.Token);

                BaseIncomingMessage incomingMessage;
                using (cancellationToken.Register(() => responseTCS.TrySetCanceled()))
                {
                    incomingMessage = await responseTCS.Task;
                }

                if (incomingMessage is ResultMessage resultMessage)
                {
                    return resultMessage;
                }
                else if (incomingMessage is PongMessage)
                {
                    return new ResultMessage() { Success = true };
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected incoming message type '{incomingMessage.Type}' for command type '{commandMessage.Type}'.");
                }
            }
            catch (TaskCanceledException)
            {
                if (commandMessage.Id > 0)
                {
                    this.incomingMessageAwaitersById.TryRemove(commandMessage.Id, out var _);
                    this.socketEventCallbacksBySubsciptionId.Remove(commandMessage.Id);
                }

                throw;
            }
        }

        private void CheckResultMessageError(BaseOutgoingMessage commandMessage, ResultMessage resultMessage)
        {
            var errorInfo = resultMessage.Error;
            if (errorInfo == null)
            {
                return;
            }

            switch (errorInfo.Code)
            {
                case ErrorCodes.Undefined:
                case ErrorCodes.InvalidFormat:
                case ErrorCodes.IdReuse:
                case ErrorCodes.HomeAssistantError:
                case ErrorCodes.NotSupported:
                    throw new InvalidOperationException($"{errorInfo.Code}: {errorInfo.Message}");
                case ErrorCodes.Unauthorized: throw new UnauthorizedAccessException(errorInfo.Message);
                case ErrorCodes.Timeout: throw new TimeoutException(errorInfo.Message);
            }

            Trace.TraceWarning($"Error response received for command [{commandMessage}] => {resultMessage.Error}");
            Debugger.Break();
        }

        /// <summary>
        /// Sends a command message and returns the <see cref="ResultMessage"/> response from the server.
        /// </summary>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the message returned by the server.
        /// </returns>
        internal async Task<ResultMessage> SendCommandWithResultAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            this.CheckIsDiposed();

            var resultMessage = await this.SendCommandAsync(commandMessage, cancellationToken);
            this.CheckResultMessageError(commandMessage, resultMessage);

            return resultMessage;
        }

        /// <summary>
        /// Sends a command message and if succeed returns the result response from the server deserialized
        /// as <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type used to deserialize the message result.</typeparam>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the message returned by the server.
        /// </returns>
        internal async Task<TResult> SendCommandWithResultAsync<TResult>(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            var resultMessage = await this.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (!resultMessage.Success)
            {
                return default;
            }

            return resultMessage.DeserializeResult<TResult>();
        }

        /// <summary>
        /// Sends a command message and returns the value indicating whether response from the server is success.
        /// </summary>
        /// <param name="commandMessage">The command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a value indicating whether response from the server is success.
        /// </returns>
        internal async Task<bool> SendCommandWithSuccessAsync(BaseOutgoingMessage commandMessage, CancellationToken cancellationToken)
        {
            var resultMessage = await this.SendCommandWithResultAsync(commandMessage, cancellationToken);
            return resultMessage.Success;
        }

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> to an event subscription.
        /// </summary>
        /// <param name="value">The event handler to subscribe.</param>
        /// <param name="eventType">The event type to subscribe to.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a value indicating whether the subscription was successful.
        /// </returns>
        internal async Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            this.CheckIsDiposed();

            // TODO: Make AddEventHandlerSubscriptionAsync and RemoveEventHandlerSubscriptionAsync thread-safe
            if (!this.socketEventSubscriptionIdByEventType.ContainsKey(eventType))
            {
                var subscribeMessage = new SubscribeEventsMessage() { EventType = eventType != Event.AnyEventFilter ? eventType : null };
                if (!await this.SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                this.socketEventSubscriptionIdByEventType.Add(eventType, new SocketEventSubscription(subscribeMessage.Id));
            }

            this.socketEventSubscriptionIdByEventType[eventType].AddSubscription(value);
            return true;
        }

        internal async Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            this.CheckIsDiposed();

            if (!this.socketEventSubscriptionIdByEventType.TryGetValue(eventType, out var socketEventSubscription))
            {
                return false;
            }

            socketEventSubscription.RemoveSubscription(value);

            if (socketEventSubscription.SubscriptionCount == 0)
            {
                var subscribeMessage = new UnsubscribeEventsMessage() { SubscriptionId = socketEventSubscription.SubscriptionId };
                if (!await this.SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                this.socketEventSubscriptionIdByEventType.Remove(eventType);
            }

            return true;
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return this.AddEventHandlerSubscriptionAsync(value, eventType.ToSnakeCase(), cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return this.RemoveEventHandlerSubscriptionAsync(value, eventType.ToSnakeCase(), cancellationToken);
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, CancellationToken cancellationToken)
        {
            return this.AddEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, CancellationToken cancellationToken)
        {
            return this.RemoveEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }
    }
}
