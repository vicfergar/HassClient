using HassClient.WS.Messages;
using System;

namespace HassClient.WS
{
    internal class SocketEventSubscription
    {
        private EventHandler<EventResultInfo> internalEventHandler;

        public uint SubscriptionId { get; set; }

        public uint SubscriptionCount { get; private set; }

        public SocketEventSubscription(uint subscriptionId)
        {
            this.SubscriptionId = subscriptionId;
        }

        public void AddSubscription(EventHandler<EventResultInfo> eventHandler)
        {
            this.internalEventHandler += eventHandler;
            this.SubscriptionCount++;
        }

        public void RemoveSubscription(EventHandler<EventResultInfo> eventHandler)
        {
            this.internalEventHandler -= eventHandler;
            this.SubscriptionCount--;
        }

        public void Invoke(EventResultInfo eventResultInfo)
        {
            this.internalEventHandler?.Invoke(this, eventResultInfo);
        }

        public void ClearAllSubscriptions()
        {
            this.internalEventHandler = null;
            this.SubscriptionCount = 0;
        }
    }
}
