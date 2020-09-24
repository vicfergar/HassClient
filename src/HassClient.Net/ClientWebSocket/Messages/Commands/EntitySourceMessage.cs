using Newtonsoft.Json;

namespace HassClient.Net.WSMessages
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
