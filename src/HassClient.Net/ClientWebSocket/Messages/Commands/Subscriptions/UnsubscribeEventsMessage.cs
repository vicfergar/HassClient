using Newtonsoft.Json;

namespace HassClient.Net.WSMessages
{
    internal class UnsubscribeEventsMessage : BaseOutgoingMessage
    {
        [JsonProperty(Required = Required.Always)]
        public uint SubscriptionId { get; set; }

        public UnsubscribeEventsMessage()
            : base("unsubscribe_events")
        {
        }
    }
}
