using Newtonsoft.Json;

namespace HassClient.Net.WSMessages
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
