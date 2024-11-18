using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;
using HassClient.WS.Tests.Mocks;
using HassClient.WS.Tests.Mocks.HassServer;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class HassClientWebSocketTests
    {
        private MockHassServerWebSocket mockServer;
        private HassClientWebSocket wsClient;
        private CancellationTokenSource connectionCTS;
        private MockEventListener connectionChangedListener;

        [SetUp]
        public void SetUp()
        {
            this.mockServer = new MockHassServerWebSocket();
            this.connectionChangedListener = new MockEventListener();
            this.connectionCTS = new CancellationTokenSource();
            this.wsClient = new HassClientWebSocket();
            this.wsClient.ConnectionStateChanged += this.connectionChangedListener.Handle;
        }

        private async Task StartMockServerAsync()
        {
            await this.mockServer.StartAsync();

            Assert.IsTrue(this.mockServer.IsStarted, "SetUp Failed: Mock server not started.");
        }

        private Task ConnectClientAsync(int retries = 0)
        {
            return this.wsClient.ConnectAsync(this.mockServer.ConnectionParameters, retries, this.connectionCTS.Token);
        }

        private async Task StartMockServerAndConnectClientAsync()
        {
            await this.StartMockServerAsync();
            await this.ConnectClientAsync();
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenConnecting()
        {
            await this.StartMockServerAndConnectClientAsync();

            Assert.AreEqual(3, connectionChangedListener.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Connecting, ConnectionStates.Authenticating, ConnectionStates.Connected }, connectionChangedListener.ReceivedEventArgs);
            Assert.IsTrue(connectionChangedListener.ReceivedEvents.All(e => this.wsClient == e.Sender));
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenClosing()
        {
            await this.StartMockServerAndConnectClientAsync();
            connectionChangedListener.Reset();
            await this.wsClient.CloseAsync();

            Assert.AreEqual(1, connectionChangedListener.HitCount);
            Assert.AreEqual(ConnectionStates.Disconnected, connectionChangedListener.ReceivedEventArgs.FirstOrDefault());
            Assert.AreEqual(this.wsClient, connectionChangedListener.ReceivedEvents.FirstOrDefault().Sender);
        }

        [Test]
        public async Task ConnectionStatusChangedWithDisconnectedRaisedWhenServerCloses()
        {
            await this.StartMockServerAndConnectClientAsync();
            connectionChangedListener.Reset();
            await this.mockServer.CloseActiveClientsAsync();

            Assert.GreaterOrEqual(connectionChangedListener.HitCount, 1);
            Assert.AreEqual(ConnectionStates.Disconnected, connectionChangedListener.ReceivedEventArgs.FirstOrDefault());
            Assert.AreEqual(this.wsClient, connectionChangedListener.ReceivedEvents.FirstOrDefault().Sender);
        }

        [Test]
        public void SendCommandWhenNotConnectedThrows()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => this.wsClient.SendCommandWithResultAsync(default, default));
        }

        [Test]
        public async Task CancelConnectOnceConnectedHasNoEffect()
        {
            await this.StartMockServerAndConnectClientAsync();

            this.connectionCTS.Cancel();

            Assert.AreEqual(ConnectionStates.Connected, this.wsClient.ConnectionState);
        }

        [Test]
        public async Task CancelConnectWhileAuthenticating()
        {
            this.mockServer.IgnoreAuthenticationMessages = true;
            await this.StartMockServerAsync();

            var connectTask = this.ConnectClientAsync();
            await this.connectionChangedListener.WaitEventArgWithTimeoutAsync(ConnectionStates.Authenticating, 1000);

            Assert.AreEqual(ConnectionStates.Authenticating, this.wsClient.ConnectionState, "SetUp Failed");

            this.connectionCTS.Cancel();

            Assert.CatchAsync<OperationCanceledException>(() => connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
            Assert.AreEqual(TaskStatus.Canceled, connectTask.Status);
        }

        [Test]
        public async Task CloseWhileConnecting()
        {
            this.mockServer.IgnoreAuthenticationMessages = true;
            await this.StartMockServerAsync();
            var connectTask = this.ConnectClientAsync();

            await this.wsClient.CloseAsync();

            Assert.CatchAsync<OperationCanceledException>(async () => await connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
        }

        [Test]
        public void ConnectWithInfiniteRetriesAndNoCancellationTokenThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => this.wsClient.ConnectAsync(new ConnectionParameters(), -1));
        }

        [Test]
        public async Task ConnectWithInvalidAuthenticationThrows()
        {
            await this.StartMockServerAsync().ConfigureAwait(false);

            var invalidParameters = new ConnectionParameters()
            {
                Endpoint = this.mockServer.ConnectionParameters.Endpoint,
                AccessToken = "Invalid_Access_Token"
            };

            await AssertExtensions.ThrowsAsync<AuthenticationException>(this.wsClient.ConnectAsync(invalidParameters));
        }

        [Test, NonParallelizable]
        public async Task ConnectWithRetriesAndInvalidAuthenticationThrows()
        {
            await this.StartMockServerAsync().ConfigureAwait(false);

            var invalidParameters = new ConnectionParameters()
            {
                Endpoint = this.mockServer.ConnectionParameters.Endpoint,
                AccessToken = "Invalid_Access_Token"
            };

            await AssertExtensions.ThrowsAsync<AuthenticationException>(this.wsClient.ConnectAsync(invalidParameters, -1, this.connectionCTS.Token));
        }

        [Test]
        public async Task ConnectOnceWhileConnectingThrows()
        {
            await this.StartMockServerAndConnectClientAsync();

            Assert.AreNotEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
            Assert.ThrowsAsync<InvalidOperationException>(() => this.ConnectClientAsync());
        }

        [Test]
        public void ConnectOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.StartMockServerAndConnectClientAsync());
        }

        [Test]
        public void CloseOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.CloseAsync());
        }

        [Test]
        public void SendCommandOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.SendCommandWithSuccessAsync(default, default));
        }

        [Test]
        public void AddEventHandlerSubscriptionOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.AddEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public void RemoveEventHandlerSubscriptionOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDisposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.RemoveEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public async Task CancelBeforeAddingEventHandlerSubscriptionThrows()
        {
            await this.StartMockServerAndConnectClientAsync();
            this.mockServer.IgnoreAuthenticationMessages = true;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var eventSubscriber = new MockEventListener();
            var subscriptionTask = this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);

            Assert.Zero(this.wsClient.HassEventSubscriptionsCount);
            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelAfterAddingEventHandlerSubscriptionThrows()
        {
            await this.StartMockServerAndConnectClientAsync();
            this.mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;
            var cancellationTokenSource = new CancellationTokenSource();
            var eventSubscriber = new MockEventListener();
            var subscriptionTask = this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);
            Assert.NotZero(this.wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(this.wsClient.HassEventSubscriptionsCount);
            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelBeforeSendingCommandThrows()
        {
            await this.StartMockServerAndConnectClientAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var sendTask = this.wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);

            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.CatchAsync<OperationCanceledException>(() => sendTask);
        }

        [Test, Repeat(200)]
        public async Task CancelAfterSendingCommandThrows()
        {
            await this.StartMockServerAndConnectClientAsync();
            this.mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;

            var cancellationTokenSource = new CancellationTokenSource();
            var sendTask = this.wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);
            Assert.NotZero(this.wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(this.wsClient.PendingRequestsCount);

            Assert.CatchAsync<OperationCanceledException>(() => sendTask);
        }

        [Test]
        public async Task Reconnection()
        {
            await this.StartMockServerAndConnectClientAsync();

            await this.mockServer.CloseActiveClientsAsync();
            await this.wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            Assert.AreEqual(ConnectionStates.Connected, this.wsClient.ConnectionState);
        }

        [Test]
        public async Task AddedEventHandlerSubscriptionsAreRestoredAfterReconnection()
        {
            var listener = new MockEventListener();
            await this.StartMockServerAndConnectClientAsync();
            var result = await this.wsClient.AddEventHandlerSubscriptionAsync(listener.Handle, default);
            Assert.IsTrue(result, "SetUp failed");

            await this.mockServer.CloseActiveClientsAsync();
            await this.wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            var entityId = "test.mock";
            await this.mockServer.RaiseStateChangedEventAsync(entityId);
            var eventResult = await listener.WaitFirstEventWithTimeoutAsync<HassEvent>(500);

            Assert.AreEqual(1, listener.HitCount);
            Assert.AreEqual(1, listener.ReceivedEventArgs.Count());
            Assert.NotNull(eventResult);
        }

        [Test]
        public async Task SendLongRunningSubscriptionCommand_WhenSuccessful_CreatesSubscription()
        {
            // Arrange
            await this.StartMockServerAndConnectClientAsync();
            var listener = new MockEventListener();

            // Act
            var subscribeMessage = new HassEventSubscribeMessage(KnownEventTypes.StateChanged.ToEventTypeString());
            var subscription = await this.wsClient.SendLongRunningSubscriptionCommandAsync(
                subscribeMessage, 
                listener.Handle, 
                CancellationToken.None);
            
            // Assert
            Assert.NotNull(subscription, "Subscription should be created");
            Assert.AreEqual(1, this.wsClient.RegisteredEventSubscriptions.Count, "Should have one subscription");
            
            // Verify the subscription receives events
            await this.mockServer.RaiseStateChangedEventAsync("light.test");
            var eventResult = await listener.WaitFirstEventWithTimeoutAsync<object>(500);
            Assert.IsNotNull(eventResult, "Should receive events");
        }

        [Test]
        public async Task SendTemporarySubscriptionCommand_WhenSuccessful_CreatesSubscription()
        {
            // Arrange
            await this.StartMockServerAndConnectClientAsync();

            var waitEventTCS = new TaskCompletionSource<IncomingEventMessage>();
            var sendEventTCS = new TaskCompletionSource<bool>();
            this.mockServer.RequestContext.OutgoingMessageInterceptor = async (msg) =>
            {
                if (msg is IncomingEventMessage incomingEventMsg)
                {
                    waitEventTCS.SetResult(incomingEventMsg);
                    await sendEventTCS.Task;
                }

                return msg;
            };

            // Act
            var subscribeMessage = new RenderTemplateMessage() { Template = "{{ states('light.test') }}" };
            var sendSubscribeTask = this.wsClient.SendTemporarySubscriptionCommandAsync(
                subscribeMessage, 
                CancellationToken.None);

            var incomingEventMsg = await waitEventTCS.Task;
            
            // Assert
            Assert.AreEqual(TaskStatus.WaitingForActivation, sendSubscribeTask.Status, "Send subscribe task should be running until the event is received");
            var temporarySubscription = this.wsClient.RegisteredEventSubscriptions.SingleOrDefault();
            Assert.IsNotNull(temporarySubscription, "Should have one subscription");
            Assert.IsFalse(temporarySubscription.IsLongRunning, "Subscription should be temporary");


            // Verify the subscription receives events
            sendEventTCS.SetResult(true);
            var result = await sendSubscribeTask;
            Assert.AreEqual(1, result.Count(), "Send subscribe task should return one event");
            Assert.AreEqual(incomingEventMsg.Event.ToString(), HassSerializer.SerializeObject(result.First()), "Event content should match");
            Assert.IsEmpty(this.wsClient.RegisteredEventSubscriptions, "Subscription should be automatically removed");
        }

        [Test]
        public async Task UnsubscribeMessage_WhenSuccessful_RemovesSubscription()
        {
            // Arrange
            await this.StartMockServerAndConnectClientAsync();
            var listener = new MockEventListener();
            var subscribeMessage = new HassEventSubscribeMessage(KnownEventTypes.StateChanged.ToEventTypeString());
            var subscription = await this.wsClient.SendLongRunningSubscriptionCommandAsync(
                subscribeMessage, 
                listener.Handle, 
                CancellationToken.None);
            
            Assert.NotNull(subscription, "Setup failed - subscription should be created");
            Assert.AreEqual(1, this.wsClient.RegisteredEventSubscriptions.Count, "Setup failed - should have one subscription");
            
            // Act
            var unsubscribeMessage = new UnsubscribeEventsMessage { Subscription = subscription.Id };
            var result = await this.wsClient.SendCommandWithSuccessAsync(unsubscribeMessage, CancellationToken.None);
            
            // Assert
            Assert.IsTrue(result, "Unsubscribe command should succeed");
            Assert.AreEqual(0, this.wsClient.RegisteredEventSubscriptions.Count, "Subscription should be removed");
            
            // Verify the subscription no longer receives events
            await this.mockServer.RaiseStateChangedEventAsync("light.test");
            var eventResult = await listener.WaitFirstEventWithTimeoutAsync<object>(500);
            Assert.IsNull(eventResult, "Should not receive events after unsubscribing");
        }

        [Test]
        public async Task ConnectionLostDuringSubscriptionRestorationTriggersReconnection()
        {
            // Setup initial connection and subscriptions
            await this.StartMockServerAndConnectClientAsync();
            var listenersByEventType = new Dictionary<KnownEventTypes, MockEventListener>();
            foreach(var eventType in Enum.GetValues<KnownEventTypes>().Take(3))
            {
                var listener = new MockEventListener();
                var sub = await this.wsClient.SendLongRunningSubscriptionCommandAsync(
                new HassEventSubscribeMessage(eventType.ToEventTypeString()), 
                    listener.Handle, 
                    default);
                Assert.NotNull(sub, $"SetUp failed: Creating subscription for {eventType}");
                listenersByEventType.Add(eventType, listener);
            }
            Assert.AreEqual(3, this.wsClient.RegisteredEventSubscriptions.Count, "SetUp failed: 3 subscriptions should be registered");


            // Configure server to drop connection during subscription restoration
            int processedSubscriptions = 0;
            this.mockServer.RequestContext.IncomingMessageInterceptor = async (msg) => 
            {
                if (msg is not ISubscribeMessage)
                {
                    return true;
                }

                processedSubscriptions++;   
                if (processedSubscriptions == 2)
                {
                    // Close the connection during second subscription restoration
                    await this.mockServer.CloseActiveClientsAsync();
                    return false; // Skip normal message handling
                }

                return true; // Process normally
            };

            // Trigger initial reconnection
            await this.mockServer.CloseActiveClientsAsync();
            
            // Wait for final successful reconnection
            await this.wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            // Verify connection and subscription are working
            Assert.AreEqual(ConnectionStates.Connected, this.wsClient.ConnectionState);
            Assert.AreEqual(3, this.wsClient.RegisteredEventSubscriptions.Count);
            Assert.AreEqual(5, processedSubscriptions, "2 + 3 subscriptions should be processed");
            
            foreach(var listener in listenersByEventType)
            {
                var testData = new {EventType=listener.Key};
                await this.mockServer.RaiseEventAsync(listener.Key, new JRaw(HassSerializer.SerializeObject(testData)));
                var eventResult = await listener.Value.WaitFirstEventWithTimeoutAsync<object>(500);

                Assert.NotNull(eventResult, "Event subscription should be restored");
            }
        }

        [Test]
        public async Task ConnectionLostDuringMultipleTimesDuringSubscriptionRestorationIsRecovered()
        {
            // Setup initial connection and subscription
            await this.StartMockServerAndConnectClientAsync();
            var listener = new MockEventListener();
            var sub = await this.wsClient.SendLongRunningSubscriptionCommandAsync(
                new HassEventSubscribeMessage(KnownEventTypes.StateChanged.ToEventTypeString()), 
                listener.Handle, 
                default);
            Assert.NotNull(sub, $"SetUp failed: Creating subscription");
            Assert.AreEqual(1, this.wsClient.RegisteredEventSubscriptions.Count, "SetUp failed: 1 subscription should be registered");

            // Configure server to drop connection during subscription restoration
            int processedSubscriptions = 0;
            this.mockServer.RequestContext.IncomingMessageInterceptor = async (msg) => 
            {
                if (msg is not ISubscribeMessage)
                {
                    return true;
                }

                processedSubscriptions++;   
                if (processedSubscriptions < 4)
                {
                    // Close the connection during the first 3 subscription restoration
                    await this.mockServer.CloseActiveClientsAsync();
                    return false; // Skip normal message handling
                }

                return true; // Process normally
            };

            // Trigger initial reconnection
            await this.mockServer.CloseActiveClientsAsync();
            
            // Wait for final successful reconnection
            await this.wsClient.WaitForConnectionAsync(TimeSpan.FromMilliseconds(200));

            // Verify connection and subscription are working
            Assert.AreEqual(ConnectionStates.Connected, this.wsClient.ConnectionState);
            Assert.AreEqual(1, this.wsClient.RegisteredEventSubscriptions.Count);
            Assert.AreEqual(4, processedSubscriptions, "3 retries + 1 success subscriptions attempts");
            
            await this.mockServer.RaiseStateChangedEventAsync("light.test");
            var eventResult = await listener.WaitFirstEventWithTimeoutAsync<object>(500);

            Assert.NotNull(eventResult, "Event subscription should be restored");
        }
    }
}
