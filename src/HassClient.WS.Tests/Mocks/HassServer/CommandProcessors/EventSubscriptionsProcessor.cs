using HassClient.Helpers;
using HassClient.Models;
using HassClient.WS.Messages;
using System.Collections.Generic;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class EventSubscriptionsProcessor : BaseCommandProcessor
    {
        private readonly Dictionary<string, List<uint>> subscribersByEventType = new Dictionary<string, List<uint>>();

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
        {
            return receivedCommand is SubscribeEventsMessage || receivedCommand is UnsubscribeEventsMessage;
        }

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            if (receivedCommand is SubscribeEventsMessage subscribeMessage)
            {
                var eventType = subscribeMessage.EventType ?? KnownEventTypes.Any.ToEventTypeString();
                if (!this.subscribersByEventType.TryGetValue(eventType, out var subscribers))
                {
                    subscribers = new List<uint>();
                    this.subscribersByEventType.Add(eventType, subscribers);

                }
                subscribers.Add(subscribeMessage.Id);
                return this.CreateResultMessageWithResult(null);
            }
            else if (receivedCommand is UnsubscribeEventsMessage unsubscribeMessage)
            {
                foreach (var item in this.subscribersByEventType.Values)
                {
                    if (item.Remove(unsubscribeMessage.SubscriptionId))
                    {
                        //success = true;
                        break;
                    }
                }
            }

            return this.CreateResultMessageWithResult(null);
        }

        public bool TryGetSubscribers(KnownEventTypes eventType, out List<uint> subscribers)
        {
            subscribers = new List<uint>();
            if (eventType != KnownEventTypes.Any &&
                this.subscribersByEventType.TryGetValue(KnownEventTypes.Any.ToEventTypeString(), out var anySubscribers))
            {
                subscribers.AddRange(anySubscribers);
            }

            if (this.subscribersByEventType.TryGetValue(eventType.ToEventTypeString(), out var typeSubscribers))
            {
                subscribers.AddRange(typeSubscribers);
            }

            return subscribers.Count > 0;
        }

        public void ClearSubscriptions()
        {
            this.subscribersByEventType.Clear();
        }
    }
}
