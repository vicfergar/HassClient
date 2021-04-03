using HassClient.Models;
using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class SubscribeEventsMessage : BaseOutgoingMessage
    {
        [JsonProperty]
        public string EventType { get; set; }

        public SubscribeEventsMessage()
            : base("subscribe_events")
        {
        }

        public SubscribeEventsMessage(string eventType)
            : this()
        {
            this.EventType = eventType;
        }

        private bool ShouldSerializeEventType() => this.EventType != Event.AnyEventFilter && this.EventType != null;
    }
}
