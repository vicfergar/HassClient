using HassClient.Net.ClientWebSocket;
using HassClient.Net.Models;
using HassClient.Net.Tests.Mocks;
using HassClient.Net.Tests.Mocks.HassServer;
using HassClient.Net.WSMessages;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
{
    public class HassClientWebSocketTests
    {
        private MockHassServerWebSocket mockServer;
        private HassClientWebSocket wsClient;
        private CancellationTokenSource connectionCTS;
        private MockEventSubscriber connectionChangedSubscriber;

        [SetUp]
        public void SetUp()
        {
            this.mockServer = new MockHassServerWebSocket();
            this.connectionChangedSubscriber = new MockEventSubscriber();
            this.connectionCTS = new CancellationTokenSource();
            this.wsClient = new HassClientWebSocket();
            wsClient.ConnectionStateChanged += connectionChangedSubscriber.Handle;
        }

        private Task StartMockServerAndConnectAsync()
        {
            if (!this.mockServer.IsStarted)
            {
                this.mockServer.Start();
            }

            return this.wsClient.ConnectAsync(this.mockServer.ServerUri, this.mockServer.AccessToken, this.connectionCTS.Token);
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenConnecting()
        {
            await this.StartMockServerAndConnectAsync();

            Assert.AreEqual(3, connectionChangedSubscriber.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Connecting, ConnectionStates.Authenticating, ConnectionStates.Connected }, connectionChangedSubscriber.ReceivedEventArgs);
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenClosing()
        {
            await this.StartMockServerAndConnectAsync();
            connectionChangedSubscriber.Reset();
            await this.wsClient.CloseAsync();

            Assert.AreEqual(1, connectionChangedSubscriber.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Disconnected }, connectionChangedSubscriber.ReceivedEventArgs);
        }

        [Test]
        public void SendCommandWhenNotConnectedThrows()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await this.wsClient.SendCommandWithResultAsync(default, default));
        }

        [Test]
        public async Task CancelConnectOnceConnectedHasNoEffect()
        {
            await this.StartMockServerAndConnectAsync();

            connectionCTS.Cancel();

            Assert.AreEqual(ConnectionStates.Connected, wsClient.ConnectionState);
        }

        [Test]
        public async Task CancelConnectWhileAuthenticating()
        {
            this.mockServer.IgnoreAuthenticationMessages = true;
            var connectTask = this.StartMockServerAndConnectAsync();
            await this.connectionChangedSubscriber.WaitEventArgWithTimeoutAsync(ConnectionStates.Authenticating, 100);

            Assert.AreEqual(ConnectionStates.Authenticating, wsClient.ConnectionState, "SetUp Failed");

            connectionCTS.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(() => connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, wsClient.ConnectionState);
            Assert.AreEqual(TaskStatus.Canceled, connectTask.Status);
        }

        [Test]
        public async Task CloseWhileConnecting()
        {
            this.mockServer.IgnoreAuthenticationMessages = true;
            var connectTask = this.StartMockServerAndConnectAsync();

            await wsClient.CloseAsync();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, wsClient.ConnectionState);
        }

        [Test]
        public async Task CloseWithCanceledTokenWhileConnectingHasNoEffect()
        {
            this.mockServer.ResponseSimulatedDelay = TimeSpan.FromMilliseconds(20);
            var connectTask = this.StartMockServerAndConnectAsync();

            var closeCTS = new CancellationTokenSource();
            closeCTS.Cancel();
            await wsClient.CloseAsync(closeCTS.Token);

            await connectTask;
            Assert.AreEqual(ConnectionStates.Connected, wsClient.ConnectionState);
        }

        [Test]
        public async Task ConnectOnceWhileConnectingThrows()
        {
            await this.StartMockServerAndConnectAsync();

            Assert.AreNotEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
            Assert.ThrowsAsync<InvalidOperationException>(() => this.StartMockServerAndConnectAsync());
        }

        [Test]
        public void ConnectOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.StartMockServerAndConnectAsync());
        }

        [Test]
        public void CloseOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.CloseAsync());
        }

        [Test]
        public void SendCommandOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.SendCommandWithSuccessAsync(default, default));
        }

        [Test]
        public void AddEventHandlerSubscriptionOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.AddEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public void RemoveEventHandlerSubscriptionOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.wsClient.RemoveEventHandlerSubscriptionAsync(default, default));
        }

        [Test]
        public async Task CancelBeforeAddingEventHandlerSubscriptionThrows()
        {
            await this.StartMockServerAndConnectAsync();
            //this.mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var eventSubscriber = new MockEventSubscriber();
            var subscriptionTask = this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);

            Assert.Zero(this.wsClient.SubscriptionsCount);
            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.ThrowsAsync<TaskCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelAfterAddingEventHandlerSubscriptionThrows()
        {
            await this.StartMockServerAndConnectAsync();
            this.mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;
            var cancellationTokenSource = new CancellationTokenSource();
            var eventSubscriber = new MockEventSubscriber();
            var subscriptionTask = this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, cancellationTokenSource.Token);
            Assert.NotZero(this.wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(this.wsClient.SubscriptionsCount);
            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.ThrowsAsync<TaskCanceledException>(() => subscriptionTask);
        }

        [Test]
        public async Task CancelBeforeSendingCommandThrows()
        {
            await this.StartMockServerAndConnectAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var sendTask = this.wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);

            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.ThrowsAsync<TaskCanceledException>(() => sendTask);
        }

        [Test]
        public async Task CancelAfterSendingCommandThrows()
        {
            await this.StartMockServerAndConnectAsync();
            this.mockServer.ResponseSimulatedDelay = TimeSpan.MaxValue;

            var cancellationTokenSource = new CancellationTokenSource();
            var sendTask = this.wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);
            Assert.NotZero(this.wsClient.PendingRequestsCount);

            cancellationTokenSource.Cancel();

            Assert.Zero(this.wsClient.PendingRequestsCount);

            Assert.ThrowsAsync<TaskCanceledException>(() => sendTask);
        }

        [Ignore("Feature not available")]
        [Test]
        public async Task AddedEventHandlerSubscriptionsAreConservedAfterReconnection()
        {
            var eventSubscriber = new MockEventSubscriber();
            await this.StartMockServerAndConnectAsync();
            var result = await this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, default);
            Assert.IsTrue(result, "SetUp failed");

            await this.wsClient.CloseAsync();
            await this.StartMockServerAndConnectAsync();

            var entityId = "test.mock";
            await this.mockServer.RaiseStateChangedEventAsync(entityId);
            var eventResult = await eventSubscriber.WaitFirstEventArgWithTimeoutAsync<StateChangedEvent>(500);

            Assert.AreEqual(1, eventSubscriber.HitCount);
            Assert.AreEqual(1, eventSubscriber.ReceivedEventArgs.Count());
            Assert.NotNull(eventResult);
            Assert.AreEqual(entityId, eventResult.EntityId);
        }
    }
}
