using HassClient.Helpers;
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
    /// <see cref="IHassWSApi"/> to send commands and subscribe for events.
    /// </summary>
    public class HassClientWebSocket : IHassClientWebSocket
    {
        private const string TAG = "[" + nameof(HassClientWebSocket) + "]";

        private const int INCONMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

        private readonly TimeSpan RetryingInterval = TimeSpan.FromSeconds(5);

        private readonly SemaphoreSlim connectionSemaphore = new SemaphoreSlim(1, 1);

        private readonly SemaphoreSlim sendingSemaphore = new SemaphoreSlim(1, 1);

        private readonly Dictionary<string, SocketEventSubscription> socketEventSubscriptionIdByEventType = new Dictionary<string, SocketEventSubscription>();
        private readonly Dictionary<uint, Action<EventResultMessage>> socketEventCallbacksBySubsciptionId = new Dictionary<uint, Action<EventResultMessage>>();
        private readonly ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>> incomingMessageAwaitersById = new ConcurrentDictionary<uint, TaskCompletionSource<BaseIncomingMessage>>();

        private ConnectionParameters connectionParameters;
        private ArraySegment<byte> receivingBuffer;

        private Channel<EventResultMessage> receivedEventsChannel;

        private ClientWebSocket socket;
        private TaskCompletionSource<bool> connectionTCS;
        private CancellationTokenSource closeConnectionCTS;
        private uint lastSentID;
        private Task socketListenerTask;
        private Task eventListenerTask;
        private ConnectionStates connectionState;

        /// <inheritdoc />
        public bool AutomaticReconnection { get; set; } = true;

        /// <inheritdoc />
        public bool IsDiposed { get; private set; }

        /// <inheritdoc />
        public ConnectionStates ConnectionState
        {
            get => this.connectionState;
            private set
            {
                if (this.connectionState != value)
                {
                    this.connectionState = value;
                    if (value == ConnectionStates.Connected)
                    {
                        this.connectionTCS.TrySetResult(true);
                    }

                    this.ConnectionStateChanged?.Invoke(this, value);
                }
            }
        }

        /// <inheritdoc />
        public bool IsReconnecting { get; private set; }

        /// <inheritdoc />
        public CalVer HAVersion { get; private set; }

        /// <inheritdoc />
        public int PendingRequestsCount => this.incomingMessageAwaitersById.Count;

        /// <inheritdoc />
        public int SubscriptionsCount => (int)this.socketEventSubscriptionIdByEventType.Values.Sum(x => x.SubscriptionCount);

        /// <inheritdoc />
        public event EventHandler<ConnectionStates> ConnectionStateChanged;

        static HassClientWebSocket()
        {
            var converters = HassSerializer.DefaultSettings.Converters;
            if (!converters.Any(x => x is MessagesConverter))
            {
                converters.Add(new MessagesConverter());
            }
        }

        /// <inheritdoc />
        public async Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default)
        {
            this.CheckIsDiposed();

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

            this.receivingBuffer = new ArraySegment<byte>(new byte[INCONMING_BUFFER_SIZE]);
            var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.closeConnectionCTS.Token, cancellationToken);
            await this.InternalConnect(connectionParameters, retries, linkedCTS.Token).ConfigureAwait(false);
            this.connectionParameters = connectionParameters;
            this.receivedEventsChannel = Channel.CreateUnbounded<EventResultMessage>();
            this.eventListenerTask = Task.Factory.StartNew(this.CreateEventListenerTask, TaskCreationOptions.LongRunning);
        }

        /// <inheritdoc />
        public async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            this.CheckIsDiposed();

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

        /// <inheritdoc />
        public Task<bool> WaitForConnectionAsync(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(timeout)} must be set greater than zero.");
            }

            return this.WaitForConnectionAsync(timeout, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"{nameof(cancellationToken)} must be set to avoid never ending wait..");
            }

            return this.WaitForConnectionAsync(TimeSpan.Zero, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> WaitForConnectionAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero && cancellationToken == CancellationToken.None)
            {
                throw new ArgumentException($"Either {nameof(timeout)} or {nameof(cancellationToken)} must be set to avoid never ending wait.");
            }

            if (this.connectionState == ConnectionStates.Connected)
            {
                return Task.FromResult(true);
            }

            if (this.connectionTCS == null)
            {
                return Task.FromResult(false);
            }

            return Task.Run(() => this.connectionTCS.Task, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

            this.CheckIsDiposed();

            // TODO: Make AddEventHandlerSubscriptionAsync and RemoveEventHandlerSubscriptionAsync thread-safe
            if (!this.socketEventSubscriptionIdByEventType.ContainsKey(eventType))
            {
                var subscribeMessage = new SubscribeEventsMessage(eventType);
                if (!await this.SendCommandWithSuccessAsync(subscribeMessage, cancellationToken))
                {
                    return false;
                }

                this.socketEventSubscriptionIdByEventType.Add(eventType, new SocketEventSubscription(subscribeMessage.Id));
            }

            this.socketEventSubscriptionIdByEventType[eventType].AddSubscription(value);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace", nameof(eventType));
            }

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
                    this.connectionTCS = new TaskCompletionSource<bool>();
                    this.socket = new ClientWebSocket();
                    await this.socket.ConnectAsync(connectionParameters.Endpoint, cancellationToken).ConfigureAwait(false);

                    this.ConnectionState = ConnectionStates.Authenticating;

                    var incomingMsg = await this.ReceiveMessage<BaseMessage>(this.receivingBuffer, cancellationToken);
                    if (incomingMsg is AuthenticationRequiredMessage authRequired)
                    {
                        var authMsg = new AuthenticationMessage();
                        authMsg.AccessToken = connectionParameters.AccessToken;
                        await this.SendMessage(authMsg, cancellationToken);

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

                            this.IsReconnecting = false;
                            this.ConnectionState = ConnectionStates.Connected;

                            Trace.WriteLine($"{TAG} Authentication succeed. Client connected {nameof(this.HAVersion)}: {this.HAVersion}");
                        }
                    }
                    else
                    {
                        throw new AuthenticationException("Unexpected message received during authentication.");
                    }

                    this.socketListenerTask = Task.Factory.StartNew(this.CreateSocketListenerTask, TaskCreationOptions.LongRunning);
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
            this.socketEventCallbacksBySubsciptionId.Clear();

            foreach (var item in this.socketEventSubscriptionIdByEventType)
            {
                this.ConnectionState = ConnectionStates.Restoring;

                var eventType = item.Key;
                while (true)
                {
                    var subscribeMessage = new SubscribeEventsMessage(eventType);
                    await this.SendMessage(subscribeMessage, closeCancellationToken);
                    var result = await this.ReceiveMessage<ResultMessage>(this.receivingBuffer, closeCancellationToken);
                    if (result.Success)
                    {
                        var socketSubscription = item.Value;
                        socketSubscription.SubscriptionId = result.Id;
                        break;
                    }
                }
            }
        }

        private async Task CreateSocketListenerTask()
        {
            var closeCancellationToken = this.closeConnectionCTS.Token;

            try
            {
                while (this.socket.State.HasFlag(WebSocketState.Open))
                {
                    var incomingMessage = await this.ReceiveMessage<BaseIncomingMessage>(this.receivingBuffer, closeCancellationToken);
                    closeCancellationToken.ThrowIfCancellationRequested();

                    if (incomingMessage is EventResultMessage eventResultMessage)
                    {
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

            if (closeCancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (this.AutomaticReconnection &&
                this.connectionParameters != null)
            {
                this.IsReconnecting = true;
                this.socketListenerTask = Task.Run(() => this.InternalConnect(this.connectionParameters, -1, closeCancellationToken));
            }
            else
            {
                this.ClearSocketResources();
            }
        }

        private async Task CreateEventListenerTask()
        {
            var channelReader = this.receivedEventsChannel.Reader;
            while (await channelReader.WaitToReadAsync(this.closeConnectionCTS.Token))
            {
                while (channelReader.TryRead(out var incomingMessage))
                {
                    if (this.socketEventCallbacksBySubsciptionId.TryGetValue(incomingMessage.Id, out var callback))
                    {
                        callback(incomingMessage);
                    }
                }
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
                this.IsReconnecting = false;

                this.connectionParameters = null;

                if (this.connectionTCS?.TrySetResult(false) == false)
                {
                    this.connectionTCS = null;
                }

                this.socket.Abort();
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
                if (this.sendingSemaphore.CurrentCount == 0)
                {
                    this.sendingSemaphore.Release();
                }
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
                var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(this.closeConnectionCTS.Token, cancellationToken);
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
            catch (OperationCanceledException)
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
    }
}
