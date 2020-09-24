using Newtonsoft.Json;

namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Response object sent by Home Assistant server as a result for a search related operation.
    /// </summary>
    public class SearchRelatedResponse
    {
        /// <summary>
        /// The identifiers of the entities related with the target entity of the search operation.
        /// </summary>
        [JsonProperty("entity")]
        public string[] EntityIds { get; set; }

        /// <summary>
        /// The identifiers of the devices related with the target entity of the search operation.
        /// </summary>
        [JsonProperty("device")]
        public string[] DeviceIds { get; set; }

        /// <summary>
        /// The identifiers of the configuration entries associated to the target entity of the search operation.
        /// </summary>
        [JsonProperty("config_entry")]
        public string[] ConfigEntryIds { get; set; }

        /// <summary>
        /// The identifiers of the automations where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("automation")]
        public string[] AutomationIds { get; set; }

        /// <summary>
        /// The identifiers of the areas where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("area")]
        public string[] AreaIds { get; set; }
    }
}
