using HassClient.Helpers;
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

        private static readonly string StateChangedEventType = KnownEventTypes.StateChanged.ToEventTypeString();

        private bool isStateChangedSubscriptionActive;

        private readonly SemaphoreSlim refreshSubscriptionsSemahore = new SemaphoreSlim(0);

        private IHassClientWebSocket clientWebSocket;

        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangedEventListener"/> class.
        /// </summary>
        /// <param name="clientWebSocket">The Home Assistant Web Socket client instance.</param>
        public StateChangedEventListener(IHassClientWebSocket clientWebSocket)
        {
            if (clientWebSocket == null)
            {
                throw new ArgumentNullException(nameof(clientWebSocket));
            }

            this.clientWebSocket = clientWebSocket;
            this.cancellationTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(
                async () =>
                {
                    while (true)
                    {
                        await this.refreshSubscriptionsSemahore.WaitAsync(this.cancellationTokenSource.Token);
                        await this.UpdateStateChangeSockedSubscription(this.cancellationTokenSource.Token);
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

        private void InternalSubscribeStatusChanged(Dictionary<string, EventHandler<StateChangedEvent>> register, string key, EventHandler<StateChangedEvent> value)
        {
            if (!register.ContainsKey(key))
            {
                register.Add(key, null);

                if (register.Count == 1)
                {
                    this.refreshSubscriptionsSemahore.Release();
                }
            }

            register[key] += value;
        }

        private void InternalUnsubscribeStatusChanged(Dictionary<string, EventHandler<StateChangedEvent>> register, string key, EventHandler<StateChangedEvent> value)
        {
            if (register.TryGetValue(key, out var subscriptions))
            {
                subscriptions -= value;
                if (subscriptions == null)
                {
                    register.Remove(key);

                    if (register.Count == 0)
                    {
                        this.refreshSubscriptionsSemahore.Release();
                    }
                }
            }
        }

        private async Task UpdateStateChangeSockedSubscription(CancellationToken cancellationToken)
        {
            var needsSubcription = this.stateChangedSubscriptionsByEntityId.Count > 0 || this.stateChangedSubscriptionsByDomain.Count > 0;
            var toggleRequired = this.isStateChangedSubscriptionActive ^ needsSubcription;
            if (toggleRequired)
            {
                var succeed = false;
                if (!this.isStateChangedSubscriptionActive)
                {
                    succeed = await this.clientWebSocket.AddEventHandlerSubscriptionAsync(this.OnStateChangeEvent, StateChangedEventType, cancellationToken);
                }
                else if (this.isStateChangedSubscriptionActive)
                {
                    succeed = await this.clientWebSocket.RemoveEventHandlerSubscriptionAsync(this.OnStateChangeEvent, StateChangedEventType, cancellationToken);
                }

                if (succeed)
                {
                    this.isStateChangedSubscriptionActive = !this.isStateChangedSubscriptionActive;
                }
                else
                {
                    // Retry
                    this.refreshSubscriptionsSemahore.Release();
                    await Task.Delay(100);
                }
            }
        }

        private void OnStateChangeEvent(object sender, EventResultInfo obj)
        {
            var stateChanged = obj.DeserializeData<StateChangedEvent>();
            if (this.stateChangedSubscriptionsByEntityId.TryGetValue(stateChanged.EntityId, out var eventHandler))
            {
                eventHandler.Invoke(this, stateChanged);
            }

            if (this.stateChangedSubscriptionsByDomain.TryGetValue(stateChanged.Domain, out eventHandler))
            {
                eventHandler.Invoke(this, stateChanged);
            }
        }
    }
}
