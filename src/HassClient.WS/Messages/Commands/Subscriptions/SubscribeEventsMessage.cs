using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class SubscribeEventsMessage : BaseOutgoingMessage
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EventType { get; set; }

        public SubscribeEventsMessage()
            : base("subscribe_events")
        {
        }
    }
}
