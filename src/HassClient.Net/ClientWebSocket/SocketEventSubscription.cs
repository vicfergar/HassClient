using HassClient.Net.WSMessages;
using System;

namespace HassClient.Net.ClientWebSocket
{
    internal class SocketEventSubscription
    {
        public readonly uint SubscriptionId;

        private EventHandler<EventResultInfo> internalEventHandler;

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
