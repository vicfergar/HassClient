using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;
using HassClient.WS.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
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

        private const int INCOMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        private readonly TimeSpan RetryingInterval = TimeSpan.FromSeconds(5);

        private readonly SemaphoreSlim connectionSemaphore = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim sendingSemaphore = new SemaphoreSlim(1, 1);

        private readonly Dictionary<string, HassEventSubscription> hassEventSubscriptionsByEventType = new Dictionary<string, HassEventSubscription>();
        private readonly Dictionary<uint, WSEventSubscription> socketEventSubscriptionsById = new Dictionary<uint, WSEventSubscription>();
        private readonly ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>> incomingMessageAwaitersById = new ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>>();

        private ConnectionParameters connectionParameters;
        private ArraySegment<byte> receivingBuffer;

        private Channel<IncomingEventMessage> receivedEventsChannel;

        private ClientWebSocket socket;
        private CancellationTokenSource closeConnectionCTS;
        private uint lastSentID;
        private Task socketListenerTask;
        private Task eventListenerTask;
        private ConnectionStates connectionState;

        /// <summary>
        /// Gets or sets a value indicating whether the client will try to reconnect when connection is lost.
        /// Default: <c>true</c>.
        /// </summary>
        public bool AutomaticReconnection { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

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
        /// Gets a value indicating whether the connection with the server has been
        /// lost and the client is trying to reconnect.
        /// </summary>
        public bool IsReconnecting { get; private set; }

        /// <summary>
        /// Gets the connected Home Assistant instance version.
        /// </summary>
        public CalVer HAVersion { get; private set; }

        /// <summary>
        /// Gets the number of requests that are pending to be attended by the server.
        /// </summary>
        public int PendingRequestsCount => this.incomingMessageAwaitersById.Count;

        /// <summary>
        /// Gets the number of event handler subscriptions.
        /// </summary>
        public int HassEventSubscriptionsCount => (int)this.hassEventSubscriptionsByEventType.Values.Sum(x => x.SubscriptionCount);

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        public event EventHandler<ConnectionStates> ConnectionStateChanged;

        /// <summary>
        /// Gets the registered event subscriptions.
        /// </summary>
        public IReadOnlyList<WSEventSubscription> RegisteredEventSubscriptions => this.socketEventSubscriptionsById.Values.ToList();

        static HassClientWebSocket()
        {
            var converters = HassSerializer.DefaultSettings.Converters;
            if (!converters.Any(x => x is MessagesConverter))
            {
                converters.Add(new MessagesConverter());
            }
        }

        /// <summary>
        /// Connects to a Home Assistant instance using the specified connection parameters.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="retries">
        /// Number of retries if connection failed. Default: 0.
        /// <para>
        /// Retries will only be performed if Home Assistant instance cannot be reached and not if:
        /// authentication fails OR
        /// invalid response from server OR
        /// connection refused by server.
        /// </para>
        /// <para>
        /// If set to <c>-1</c>, this method will try indefinitely until connection succeed or
        /// cancellation is requested. Therefore, <paramref name="cancellationToken"/> must be set
        /// to a value different to <see cref="CancellationToken.None"/> in that case.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default)
        {
            this.CheckIsDisposed();

            if (retries < 0 &&
                cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException(
                    nameof(cancellationToken),
                    $"{nameof(cancellationToken)} must be set to a value different to {nameof(CancellationToken.None)} when retrying indefinitely");
            }

            if (this.ConnectionState != ConnectionStates.Disconnected)
            {
                throw new InvalidOperationException($"{nameof(HassClientWebSocket)} is not disconnected.");
            }

            this.closeConnectionCTS = new CancellationTokenSource();

            this.receivingBuffer = new ArraySegment<byte>(new byte[INCOMING_BUFFER_SIZE]);
            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.closeConnectionCTS.Token, cancellationToken);
            await this.InternalConnect(connectionParameters, retries, linkedCTS.Token).ConfigureAwait(false);
            this.connectionParameters = connectionParameters;
            this.receivedEventsChannel = Channel.CreateUnbounded<IncomingEventMessage>();
            this.eventListenerTask = Task.Factory.StartNew(this.CreateEventListenerTask, TaskCreationOptions.LongRunning);
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
            this.CheckIsDisposed();

            if (this.ConnectionState == ConnectionStates.Disconnected)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            this.closeConnectionCTS?.Cancel();
            await this.connectionSemaphore.WaitAsync();

            this.ClearSocketResources();
            this.connectionSemaphore.Release();
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        public Task<bool> WaitForConnectionAsync(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(timeout)} must be set greater than zero.");
            }

            return this.WaitForConnectionAsync(timeout, CancellationToken.None);
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        public Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} must be set to avoid never ending wait..");
            }

            return this.WaitForConnectionAsync(TimeSpan.Zero, cancellationToken);
        }

        /// <summary>
        /// Waits until the client state changed to connected.
        /// <para>
        /// Either <paramref name="timeout"/> or <paramref name="cancellationToken"/> must be set to avoid never ending wait.
        /// </para>
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <c>true</c>
        /// if the client has been connected; otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> WaitForConnectionAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero && cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"Either {nameof(timeout)} or {nameof(cancellationToken)} must be set to avoid never ending wait.");
            }

            if (this.connectionState == ConnectionStates.Connected)
            {
                return true;
            }

            var tcs = new TaskCompletionSource<bool>();
            void OnConnectionStateChanged(object s, ConnectionStates state)
            {
                if (state == ConnectionStates.Connected)
                {
                    tcs.TrySetResult(true);
                }
            }

            var timeoutCTS = timeout > TimeSpan.Zero
                ? new CancellationTokenSource(timeout)
                : new CancellationTokenSource();

            using (timeoutCTS)
            using (var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                timeoutCTS.Token,
                cancellationToken))
            {
                try
                {
                    this.ConnectionStateChanged += OnConnectionStateChanged;
                    return await Task.Run(() => tcs.Task, linkedCTS.Token);
                }
                catch (OperationCanceledException)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        return false; // Timeout occurred
                    }

                    throw; // Cancellation was requested through cancellationToken
                }
                finally
                {
                    this.ConnectionStateChanged -= OnConnectionStateChanged;
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;
                this.ClearSocketResources();

                foreach (var item in this.hassEventSubscriptionsByEventType.Values)
                {
                    item.ClearAllSubscriptions();
                }

                this.hassEventSubscriptionsByEventType.Clear();
            }
        }

        private async Task InternalConnect(ConnectionParameters connectionParameters, int retries, CancellationToken cancellationToken)
        {
            this.ConnectionState = ConnectionStates.Connecting;

            var retry = false;
            do
            {
                await this.connectionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    retry = false;
                    cancellationToken.ThrowIfCancellationRequested();

                    this.lastSentID = 0;
                    this.socket = new ClientWebSocket();
                    await this.socket.ConnectAsync(connectionParameters.Endpoint, cancellationToken).ConfigureAwait(false);

                    this.ConnectionState = ConnectionStates.Authenticating;

                    var incomingMsg = await this.ReceiveMessage<BaseMessage>(this.receivingBuffer, cancellationToken);
                    if (incomingMsg is AuthenticationRequiredMessage authRequired)
                    {
                        var authMsg = new AuthenticationMessage();
                        authMsg.AccessToken = connectionParameters.AccessToken;
                        await this.SendMessageAsync(authMsg, cancellationToken);

                        incomingMsg = await this.ReceiveMessage<BaseMessage>(this.receivingBuffer, cancellationToken);
                        if (incomingMsg is AuthenticationInvalidMessage authenticationInvalid)
                        {
                            throw new AuthenticationException($"{TAG} Invalid authentication: {authenticationInvalid.Message}");
                        }
                        else if (incomingMsg is AuthenticationOkMessage authenticationOk)
                        {
                            this.HAVersion = CalVer.Create(authenticationOk.HAVersion);

                            if (this.IsReconnecting)
                            {
                                await this.RestoreEventsSubscriptionsAsync(cancellationToken);
                            }
                            else
                            {
                                this.socketListenerTask = Task.Factory.StartNew(this.CreateSocketListenerTask, TaskCreationOptions.LongRunning);
                            }

                            this.IsReconnecting = false;
                            this.ConnectionState = ConnectionStates.Connected;

                            Trace.WriteLine($"{TAG} Authentication succeed. Client connected {nameof(this.HAVersion)}: {this.HAVersion}");
                        }
                    }
                    else
                    {
                        throw new AuthenticationException("Unexpected message received during authentication.");
                    }
                }
                catch (Exception ex)
                {
                    retry = (retries < 0 || retries-- > 0) && ex is WebSocketException;

                    if (retry)
                    {
                        Trace.WriteLine($"{TAG} Connecting attempt failed. Retrying in {this.RetryingInterval.TotalSeconds} seconds...");
                        await Task.Delay(this.RetryingInterval);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (!retry && this.ConnectionState != ConnectionStates.Connected)
                    {
                        this.ClearSocketResources();
                    }

                    if (this.connectionSemaphore.CurrentCount == 0)
                    {
                        this.connectionSemaphore.Release();
                    }
                }
            }
            while (retry);
        }

        private async Task RestoreEventsSubscriptionsAsync(CancellationToken closeCancellationToken)
        {
            var subscriptionsToRestore = this.socketEventSubscriptionsById
                .Values
                .Where(x => x.IsLongRunning)
                .ToList();
            var restoredSubscriptions = new List<WSEventSubscription>();

            foreach (var subscription in subscriptionsToRestore)
            {
                this.ConnectionState = ConnectionStates.Restoring;
                await this.SendMessageAsync(subscription.SubscribeMessage, closeCancellationToken);
                var result = await this.ReceiveMessage<ResultMessage>(this.receivingBuffer, closeCancellationToken);
                if (result.Success)
                {
                    if (subscription.SubscribeMessage is HassEventSubscribeMessage hassEventSubscribeMessage)
                    {
                        string eventType = hassEventSubscribeMessage.EventType;
                        if (this.hassEventSubscriptionsByEventType.TryGetValue(eventType, out var hassEventSubscription))
                        {
                            hassEventSubscription.SubscriptionId = result.Id;
                        }
                    }

                    subscription.Id = result.Id;
                    restoredSubscriptions.Add(subscription);
                }
                else
                {
                    Trace.TraceError($"{TAG} Failed to restore subscription {subscription.SubscribeMessage}");
                }
            }

            this.socketEventSubscriptionsById.Clear();
            foreach (var subscription in restoredSubscriptions)
            {
                this.socketEventSubscriptionsById.Add(subscription.Id, subscription);
            }
        }

        private async Task CreateSocketListenerTask()
        {
            while (!this.closeConnectionCTS.Token.IsCancellationRequested)
            {
                try
                {
                    while (this.socket.State.HasFlag(WebSocketState.Open))
                    {
                        var incomingMessage = await this.ReceiveMessage<BaseIncomingMessage>(this.receivingBuffer, this.closeConnectionCTS.Token);
                        this.closeConnectionCTS.Token.ThrowIfCancellationRequested();

                        if (incomingMessage is IncomingEventMessage eventResultMessage)
                        {
                            Debug.WriteLine($"{TAG} Event message received {eventResultMessage}");
                            if (!this.receivedEventsChannel.Writer.TryWrite(eventResultMessage))
                            {
                                Trace.TraceWarning($"{TAG} {nameof(this.receivedEventsChannel)} is full. One event message will discarded.");
                            }
                        }
                        else if (incomingMessage is PongMessage ||
                                incomingMessage is ResultMessage)
                        {
                            Debug.WriteLine($"{TAG} Command message received {incomingMessage}");
                            if (this.incomingMessageAwaitersById.TryRemove(incomingMessage.Id, out var responseTCS))
                            {
                                responseTCS.SetResult(incomingMessage);
                            }
                            else
                            {
                                Trace.TraceError($"{TAG} No awaiter found for incoming message {incomingMessage}. Message will be discarded.");
                            }
                        }
                        else if (this.socket.State.HasFlag(WebSocketState.Open))
                        {
                            Trace.TraceError($"{TAG} Unexpected message type received: {incomingMessage}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Trace.WriteLine($"{TAG} Connection stopped for cancellation.");
                    return;
                }
                catch (WebSocketException)
                {
                }

                this.ConnectionState = ConnectionStates.Disconnected;
                Trace.WriteLine($"{TAG} Connection ended {this.socket.CloseStatus?.ToString() ?? this.socket.State.ToString()}");

                if (this.closeConnectionCTS.Token.IsCancellationRequested)
                {
                    return;
                }

                if (this.AutomaticReconnection &&
                    this.connectionParameters != null)
                {
                    this.IsReconnecting = true;
                    await this.InternalConnect(this.connectionParameters, -1, this.closeConnectionCTS.Token);
                }
                else
                {
                    this.ClearSocketResources();
                    break;
                }
            }
        }

        private async Task CreateEventListenerTask()
        {
            var channelReader = this.receivedEventsChannel.Reader;
            while (await channelReader.WaitToReadAsync(this.closeConnectionCTS.Token))
            {
                while (channelReader.TryRead(out var incomingMessage))
                {
                    if (this.socketEventSubscriptionsById.TryGetValue(incomingMessage.Id, out var subscription))
                    {
                        subscription.Callback(incomingMessage);
                    }
                }
            }
        }

        private void CheckIsDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(HassClientWebSocket));
            }
        }

        private void ClearSocketResources()
        {
            if (this.ConnectionState != ConnectionStates.Disconnected)
            {
                this.ConnectionState = ConnectionStates.Disconnected;
                this.IsReconnecting = false;

                this.connectionParameters = null;

                this.socket.Abort();
                this.socket.Dispose();

                this.socketEventSubscriptionsById.Clear();
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
                Debug.WriteLine($"{TAG} Raw message received: {rcvMsg}");
                return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
            }
            catch (JsonException)
            {
                throw;
            }
        }

        private Task SendMessageAsync(BaseMessage message, CancellationToken cancellationToken)
        {
            return this.SendMessageAsync(message, null, cancellationToken);
        }

        private async Task SendMessageAsync(BaseMessage message, TaskCompletionSource<BaseIncomingMessage> responseTCS, CancellationToken cancellationToken)
        {
            try
            {
                object toSerialize = message;
                await this.sendingSemaphore.WaitAsync(cancellationToken);
                if (message is BaseIdentifiableMessage identifiableMessage)
                {
                    identifiableMessage.Id = ++this.lastSentID;

                    if (message is RawCommandMessage rawCommand &&
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
                Debug.WriteLine($"{TAG} Raw message sent: {sendMsg}");
            }
            finally
            {
                if (this.sendingSemaphore.CurrentCount == 0)
                {
                    this.sendingSemaphore.Release();
                }
            }
        }

        private void ProcessReceivedEventSubscriptionMessage(IncomingEventMessage eventResultMessage)
        {
            // TODO: Migrate from WSClient to HassClient
            var eventResultInfo = eventResultMessage.DeserializeEvent<HassEvent>();
            if (this.hassEventSubscriptionsByEventType.TryGetValue(eventResultInfo.EventType, out var socketEventSubscription) &&
                socketEventSubscription.SubscriptionId == eventResultMessage.Id)
            {
                socketEventSubscription.Invoke(eventResultInfo);
            }

            if (this.hassEventSubscriptionsByEventType.TryGetValue(Event.AnyEventFilter, out socketEventSubscription) &&
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
                var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.closeConnectionCTS.Token, cancellationToken);
                var responseTCS = new TaskCompletionSource<BaseIncomingMessage>(linkedCTS.Token);
                await this.SendMessageAsync(commandMessage, responseTCS, linkedCTS.Token);

                BaseIncomingMessage incomingMessage;
                using (cancellationToken.Register(() => responseTCS.TrySetCanceled()))
                {
                    incomingMessage = await responseTCS.Task;
                }

                if (incomingMessage is ResultMessage resultMessage)
                {
                    if (resultMessage.Success &&
                        commandMessage is BaseUnsubscribeMessage unsubscribeMessage)
                    {
                        this.socketEventSubscriptionsById.Remove(unsubscribeMessage.Subscription);
                    }

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
            catch (OperationCanceledException)
            {
                // Move this to SendMessageAsync
                if (commandMessage.Id > 0)
                {
                    this.incomingMessageAwaitersById.TryRemove(commandMessage.Id, out var _);
                }

                throw;
            }
        }

        private async Task<WSEventSubscription> SendSubscribeCommandAsync<TSubscribeMessage>(TSubscribeMessage commandMessage, Action<IncomingEventMessage> eventCallback, CancellationToken cancellationToken)
            where TSubscribeMessage : BaseOutgoingMessage, ISubscribeMessage
        {
            this.CheckIsDisposed();

            try
            {
                var resultMessage = await this.SendCommandAsync(commandMessage, cancellationToken);
                this.CheckResultMessageError(commandMessage, resultMessage);

                if (!resultMessage.Success)
                {
                    return null;
                }

                var subscription = new WSEventSubscription(commandMessage, eventCallback);
                this.socketEventSubscriptionsById.Add(commandMessage.Id, subscription);
                return subscription;
            }
            catch
            {
                this.socketEventSubscriptionsById.Remove(commandMessage.Id);
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
                case ErrorCodes.UnknownError:
                    throw new Exception($"Unknown error occurred: {errorInfo.Message}");
                case ErrorCodes.NotFound:
                    // Handle NotFound without throwing an exception
                    Trace.TraceWarning($"NotFound error for command [{commandMessage}]: {errorInfo.Message}");
                    break;
                default:
                    Trace.TraceWarning($"Unhandled error code [{errorInfo.Code}] for command [{commandMessage}]: {errorInfo.Message}");
                    break;
            }

            Debugger.Break();
        }

        private void CheckNotASubscriptionMessage(BaseOutgoingMessage commandMessage)
        {
            if (commandMessage is ISubscribeMessage)
            {
                throw new InvalidOperationException("This method is not supported for subscription messages.");
            }
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
            this.CheckIsDisposed();
            this.CheckNotASubscriptionMessage(commandMessage);

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
        /// Sends a long-running subscription command and returns a value indicating whether the subscription was successful.
        /// </summary>
        /// <typeparam name="TEventData">The type used to deserialize the event data.</typeparam>
        /// <param name="subscribeMessage">The subscription command message to be sent.</param>
        /// <param name="eventCallback">The callback to handle incoming event messages.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is a value indicating whether the subscription was successful.</returns>
        public Task<WSEventSubscription> SendLongRunningSubscriptionCommandAsync<TEventData>(BaseSubscribeMessage<TEventData> subscribeMessage, Action<IncomingEventMessage> eventCallback, CancellationToken cancellationToken)
        {
            return this.SendSubscribeCommandAsync(subscribeMessage, eventCallback, cancellationToken);
        }

        /// <summary>
        /// Sends a temporary subscription command and returns a list of the received event results.
        /// </summary>
        /// <typeparam name="TEventData">The type used to deserialize the event data.</typeparam>
        /// <param name="commandMessage">The subscription command message to be sent.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a list of the received event results.
        /// </returns>
        public async Task<IEnumerable<TEventData>> SendTemporarySubscriptionCommandAsync<TEventData>(BaseTemporarySubscribeMessage<TEventData> commandMessage, CancellationToken cancellationToken)
        {
            WSEventSubscription subscription = null;
            try
            {
                var receivedEvents = new List<TEventData>();
                var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.closeConnectionCTS.Token, cancellationToken);
                var responseTCS = new TaskCompletionSource<bool>(linkedCTS.Token);
                var eventCallback = new Action<IncomingEventMessage>(eventResultMessage =>
                {
                    var eventData = eventResultMessage.DeserializeEvent<TEventData>();
                    receivedEvents.Add(eventData);

                    if (commandMessage.IsLastEvent(eventData))
                    {
                        responseTCS.SetResult(true);
                    }
                });

                subscription = await this.SendSubscribeCommandAsync(commandMessage, eventCallback, cancellationToken);
                if (subscription == null)
                {
                    return Enumerable.Empty<TEventData>();
                }

                await responseTCS.Task;

                return receivedEvents;
            }
            finally
            {
                if (subscription != null)
                {
                    this.socketEventSubscriptionsById.Remove(subscription.Id);
                }
            }
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
        internal async Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

            this.CheckIsDisposed();

            // TODO: Make AddEventHandlerSubscriptionAsync and RemoveEventHandlerSubscriptionAsync thread-safe
            if (!this.hassEventSubscriptionsByEventType.ContainsKey(eventType))
            {
                var subscribeMessage = new HassEventSubscribeMessage(eventType);
                var subscription = await this.SendLongRunningSubscriptionCommandAsync(subscribeMessage, this.ProcessReceivedEventSubscriptionMessage, cancellationToken);
                if (subscription == null)
                {
                    return false;
                }

                this.hassEventSubscriptionsByEventType.Add(eventType, new HassEventSubscription(this, subscription.Id));
            }

            this.hassEventSubscriptionsByEventType[eventType].AddSubscription(value);
            return true;
        }

        internal async Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

            this.CheckIsDisposed();

            if (!this.hassEventSubscriptionsByEventType.TryGetValue(eventType, out var socketEventSubscription))
            {
                return false;
            }

            if (!socketEventSubscription.RemoveSubscription(value))
            {
                return false;
            }

            if (socketEventSubscription.SubscriptionCount == 0)
            {
                var subscribeMessage = new UnsubscribeEventsMessage() { Subscription = socketEventSubscription.SubscriptionId };
                if (!await this.SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                this.hassEventSubscriptionsByEventType.Remove(eventType);
            }

            return true;
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return this.AddEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, KnownEventTypes eventType, CancellationToken cancellationToken)
        {
            return this.RemoveEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        internal Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, CancellationToken cancellationToken)
        {
            return this.AddEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }

        internal Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<HassEvent> value, CancellationToken cancellationToken)
        {
            return this.RemoveEventHandlerSubscriptionAsync(value, Event.AnyEventFilter, cancellationToken);
        }
    }
}
