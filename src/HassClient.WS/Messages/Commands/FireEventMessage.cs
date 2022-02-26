using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class FireEventMessage : BaseOutgoingMessage
    {
        [JsonProperty(Required = Required.Always)]
        public string EventType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object EventData { get; set; }

        public FireEventMessage()
            : base("fire_event")
        {
        }

        public FireEventMessage(string eventType, object eventData)
            : this()
        {
            this.EventType = eventType;
            this.EventData = eventData;
        }
    }
}
