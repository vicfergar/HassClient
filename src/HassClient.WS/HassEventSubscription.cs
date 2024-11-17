using HassClient.WS.Messages;
using System;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an active subscription to a Home Assistant event.
    /// </summary>
    internal class HassEventSubscription
    {
        private readonly object sender;

        private EventHandler<HassEvent> internalEventHandler;

        public uint SubscriptionId { get; set; }

        public uint SubscriptionCount { get; private set; }

        public HassEventSubscription(object sender, uint subscriptionId)
        {
            this.sender = sender;
            this.SubscriptionId = subscriptionId;
        }

        public void AddSubscription(EventHandler<HassEvent> eventHandler)
        {
            this.internalEventHandler += eventHandler;
            this.SubscriptionCount++;
        }

        public bool RemoveSubscription(EventHandler<HassEvent> eventHandler)
        {
            if (this.internalEventHandler == null)
            {
                return false;
            }

            var beforeCount = this.internalEventHandler.GetInvocationList().Length;
            this.internalEventHandler -= eventHandler;
            var afterCount = this.internalEventHandler?.GetInvocationList().Length ?? 0;
            if (beforeCount > afterCount)
            {
                this.SubscriptionCount--;
                return true;
            }

            return false;
        }

        public void Invoke(HassEvent eventResultInfo)
        {
            this.internalEventHandler?.Invoke(this.sender, eventResultInfo);
        }

        public void ClearAllSubscriptions()
        {
            this.internalEventHandler = null;
            this.SubscriptionCount = 0;
        }
    }
}
