using HassClient.Models;
using HassClient.WS.Tests.Mocks;
using HassClient.WS.Tests.Mocks.HassServer;
using HassClient.WS.Messages;
using NUnit.Framework;
using System;
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
        private MockEventSubscriber connectionChangedSubscriber;

        [SetUp]
        public void SetUp()
        {
            this.mockServer = new MockHassServerWebSocket();
            this.connectionChangedSubscriber = new MockEventSubscriber();
            this.connectionCTS = new CancellationTokenSource();
            this.wsClient = new HassClientWebSocket();
            this.wsClient.ConnectionStateChanged += connectionChangedSubscriber.Handle;
        }

        private async Task StartMockServerAsync()
        {
            await this.mockServer.StartAsync();

            Assert.IsTrue(this.mockServer.IsStarted, "SetUp Failed: Mock server not started.");
        }

        private Task ConnectClientAsync()
        {
            return this.wsClient.ConnectAsync(this.mockServer.ServerUri, this.mockServer.AccessToken, this.connectionCTS.Token);
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

            Assert.AreEqual(3, connectionChangedSubscriber.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Connecting, ConnectionStates.Authenticating, ConnectionStates.Connected }, connectionChangedSubscriber.ReceivedEventArgs);
        }

        [Test]
        public async Task ConnectionStatusChangedRaisedWhenClosing()
        {
            await this.StartMockServerAndConnectClientAsync();
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
            await this.connectionChangedSubscriber.WaitEventArgWithTimeoutAsync(ConnectionStates.Authenticating, 1000);

            Assert.AreEqual(ConnectionStates.Authenticating, this.wsClient.ConnectionState, "SetUp Failed");

            this.connectionCTS.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(() => connectTask);
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

            Assert.ThrowsAsync<TaskCanceledException>(async () => await connectTask);
            Assert.AreEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
        }

        [Test]
        public async Task CloseWithCanceledTokenWhileConnectingHasNoEffect()
        {
            this.mockServer.ResponseSimulatedDelay = TimeSpan.FromMilliseconds(20);
            var connectTask = this.StartMockServerAndConnectClientAsync();

            var closeCTS = new CancellationTokenSource();
            closeCTS.Cancel();
            await this.wsClient.CloseAsync(closeCTS.Token);

            await connectTask;
            Assert.AreEqual(ConnectionStates.Connected, this.wsClient.ConnectionState);
        }

        [Test]
        public async Task ConnectOnceWhileConnectingThrows()
        {
            await this.StartMockServerAndConnectClientAsync();

            Assert.AreNotEqual(ConnectionStates.Disconnected, this.wsClient.ConnectionState);
            Assert.ThrowsAsync<InvalidOperationException>(() => this.StartMockServerAndConnectClientAsync());
        }

        [Test]
        public void ConnectOnceDisposedThrows()
        {
            this.wsClient.Dispose();

            Assert.IsTrue(this.wsClient.IsDiposed);
            Assert.ThrowsAsync<ObjectDisposedException>(() => this.StartMockServerAndConnectClientAsync());
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
            await this.StartMockServerAndConnectClientAsync();
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
            await this.StartMockServerAndConnectClientAsync();
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
            await this.StartMockServerAndConnectClientAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var sendTask = this.wsClient.SendCommandWithSuccessAsync(new PingMessage(), cancellationTokenSource.Token);

            Assert.Zero(this.wsClient.PendingRequestsCount);
            Assert.ThrowsAsync<TaskCanceledException>(() => sendTask);
        }

        [Test]
        public async Task CancelAfterSendingCommandThrows()
        {
            await this.StartMockServerAndConnectClientAsync();
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
            await this.StartMockServerAndConnectClientAsync();
            var result = await this.wsClient.AddEventHandlerSubscriptionAsync(eventSubscriber.Handle, default);
            Assert.IsTrue(result, "SetUp failed");

            await this.wsClient.CloseAsync();
            await this.StartMockServerAndConnectClientAsync();

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
