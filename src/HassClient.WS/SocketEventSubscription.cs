using HassClient.WS.Messages;
using System;

namespace HassClient.WS
{
    internal class SocketEventSubscription
    {
        private readonly object sender;

        private EventHandler<EventResultInfo> internalEventHandler;

        public uint SubscriptionId { get; set; }

        public uint SubscriptionCount { get; private set; }

        public SocketEventSubscription(object sender, uint subscriptionId)
        {
            this.sender = sender;
            this.SubscriptionId = subscriptionId;
        }

        public void AddSubscription(EventHandler<EventResultInfo> eventHandler)
        {
            this.internalEventHandler += eventHandler;
            this.SubscriptionCount++;
        }

        public bool RemoveSubscription(EventHandler<EventResultInfo> eventHandler)
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

        public void Invoke(EventResultInfo eventResultInfo)
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
