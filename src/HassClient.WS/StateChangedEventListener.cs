using HassClient.Models;
using HassClient.WS.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Helper class to handle state changed event subscriptions from multiple consumers.
    /// </summary>
    public class StateChangedEventListener : IDisposable
    {
        private readonly Dictionary<string, EventHandler<StateChangedEvent>> stateChangedSubscriptionsByEntityId = new Dictionary<string, EventHandler<StateChangedEvent>>();
        private readonly Dictionary<string, EventHandler<StateChangedEvent>> stateChangedSubscriptionsByDomain = new Dictionary<string, EventHandler<StateChangedEvent>>();

        private readonly SemaphoreSlim refreshSubscriptionsSemaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly object subscriptionChangeLock = new object();

        private HassClientWebSocket clientWebSocket;

        private bool isStateChangedSubscriptionActive;

        private Task refreshSubscriptionsTask;

        /// <summary>
        /// Initialization method of the <see cref="StateChangedEventListener"/>.
        /// </summary>
        /// <param name="clientWebSocket">The Home Assistant Web Socket client instance.</param>
        public void Initialize(HassClientWebSocket clientWebSocket)
        {
            if (this.clientWebSocket != null)
            {
                throw new InvalidOperationException($"{nameof(StateChangedEventListener)} is already initialized");
            }

            if (clientWebSocket == null)
            {
                throw new ArgumentNullException(nameof(clientWebSocket));
            }

            this.clientWebSocket = clientWebSocket;

            this.refreshSubscriptionsTask = Task.Factory.StartNew(
                async () =>
                {
                    while (!this.cancellationTokenSource.IsCancellationRequested)
                    {
                        await this.refreshSubscriptionsSemaphore.WaitAsync(this.cancellationTokenSource.Token);
                        await this.UpdateStateChangeSocketSubscriptionAsync(this.cancellationTokenSource.Token);
                    }
                }, TaskCreationOptions.LongRunning);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Add an <see cref="EventHandler{StateChangedEvent}"/> subscription for an specific entity id.
        /// </summary>
        /// <param name="entityId">The id of the entity to track.</param>
        /// <param name="value">The <see cref="EventHandler{StateChangedEvent}"/> to be included.</param>
        public void SubscribeEntityStatusChanged(string entityId, EventHandler<StateChangedEvent> value)
        {
            this.InternalSubscribeStatusChanged(this.stateChangedSubscriptionsByEntityId, entityId, value);
        }

        /// <summary>
        /// Removes an already registered <see cref="EventHandler{StateChangedEvent}"/>.
        /// </summary>
        /// <param name="entityId">The id of the tracked entity.</param>
        /// <param name="value">The <see cref="EventHandler{StateChangedEvent}"/> to be removed.</param>
        public void UnsubscribeEntityStatusChanged(string entityId, EventHandler<StateChangedEvent> value)
        {
            this.InternalUnsubscribeStatusChanged(this.stateChangedSubscriptionsByEntityId, entityId, value);
        }

        /// <summary>
        /// Add an <see cref="EventHandler{StateChangedEvent}"/> subscription for an specific domain.
        /// </summary>
        /// <param name="domain">The domain to track.</param>
        /// <param name="value">The <see cref="EventHandler{StateChangedEvent}"/> to be included.</param>
        public void SubscribeDomainStatusChanged(string domain, EventHandler<StateChangedEvent> value)
        {
            this.InternalSubscribeStatusChanged(this.stateChangedSubscriptionsByDomain, domain, value);
        }

        /// <summary>
        /// Removes an already registered <see cref="EventHandler{StateChangedEvent}"/>.
        /// </summary>
        /// <param name="domain">The tracked domain.</param>
        /// <param name="value">The <see cref="EventHandler{StateChangedEvent}"/> to be removed.</param>
        public void UnsubscribeDomainStatusChanged(string domain, EventHandler<StateChangedEvent> value)
        {
            this.InternalUnsubscribeStatusChanged(this.stateChangedSubscriptionsByDomain, domain, value);
        }

        /// <summary>
        /// Wait for any pending subscription change to be completed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{Boolean}"/> representing the asynchronous operation.
        /// Returns <c>true</c> if no subscription change is pending or it has been completed; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> WaitForSubscriptionCompletedAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsSubscriptionChangeRequired())
            {
                return true;
            }

            try
            {
                while (true)
                {
                    var result = await this.refreshSubscriptionsSemaphore.WaitAsync(TimeSpan.FromMilliseconds(100), cancellationToken);
                    if (result)
                    {
                        this.refreshSubscriptionsSemaphore.Release();
                    }

                    if (!this.IsSubscriptionChangeRequired())
                    {
                        return true;
                    }
                    else
                    {
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        private void InternalSubscribeStatusChanged(Dictionary<string, EventHandler<StateChangedEvent>> register, string key, EventHandler<StateChangedEvent> value)
        {
            lock (this.subscriptionChangeLock)
            {
                if (!register.ContainsKey(key))
                {
                    register.Add(key, null);

                    if (register.Count == 1)
                    {
                        this.refreshSubscriptionsSemaphore.Release();
                    }
                }

                register[key] += value;
            }
        }

        private void InternalUnsubscribeStatusChanged(Dictionary<string, EventHandler<StateChangedEvent>> register, string key, EventHandler<StateChangedEvent> value)
        {
            lock (this.subscriptionChangeLock)
            {
                if (register.ContainsKey(key))
                {
                    register[key] -= value;
                    if (register[key] == null)
                    {
                        register.Remove(key);

                        if (register.Count == 0)
                        {
                            this.refreshSubscriptionsSemaphore.Release();
                        }
                    }
                }
            }
        }

        private bool IsSubscriptionChangeRequired()
        {
            lock (this.subscriptionChangeLock)
            {
                var needsSubscription = this.stateChangedSubscriptionsByEntityId.Count > 0 || this.stateChangedSubscriptionsByDomain.Count > 0;
                return this.isStateChangedSubscriptionActive ^ needsSubscription;
            }
        }

        private async Task UpdateStateChangeSocketSubscriptionAsync(CancellationToken cancellationToken)
        {
            if (!this.IsSubscriptionChangeRequired())
            {
                return;
            }

            var succeed = false;
            if (!this.isStateChangedSubscriptionActive)
            {
                succeed = await this.clientWebSocket.AddEventHandlerSubscriptionAsync(this.OnStateChangeEvent, KnownEventTypes.StateChanged, cancellationToken);
            }
            else if (this.isStateChangedSubscriptionActive)
            {
                succeed = await this.clientWebSocket.RemoveEventHandlerSubscriptionAsync(this.OnStateChangeEvent, KnownEventTypes.StateChanged, cancellationToken);
            }

            if (succeed)
            {
                this.isStateChangedSubscriptionActive = !this.isStateChangedSubscriptionActive;
            }
            else
            {
                // Retry
                this.refreshSubscriptionsSemaphore.Release();
                await Task.Delay(100);
            }
        }

        private void OnStateChangeEvent(object sender, EventResultInfo obj)
        {
            var stateChanged = obj.DeserializeData<StateChangedEvent>();
            EventHandler<StateChangedEvent> entityHandler = null;
            EventHandler<StateChangedEvent> domainHandler = null;

            lock (this.subscriptionChangeLock)
            {
                if (this.stateChangedSubscriptionsByEntityId.TryGetValue(stateChanged.EntityId, out var eh))
                {
                    entityHandler = eh;
                }

                if (this.stateChangedSubscriptionsByDomain.TryGetValue(stateChanged.Domain, out var dh))
                {
                    domainHandler = dh;
                }
            }

            entityHandler?.Invoke(this, stateChanged);
            domainHandler?.Invoke(this, stateChanged);
        }
    }
}
