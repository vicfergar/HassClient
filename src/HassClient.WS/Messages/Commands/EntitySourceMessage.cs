using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class EntitySourceMessage : BaseOutgoingMessage
    {
        [JsonProperty("entity_id", NullValueHandling = NullValueHandling.Ignore)]
        public string[] EntityIds { get; set; }

        public EntitySourceMessage()
            : base("entity/source")
        {
        }
    }
}
