using System;
using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Response object sent by Home Assistant server as a result for a search related operation.
    /// </summary>
    public class SearchRelatedResponse
    {
        /// <summary>
        /// The identifiers of the areas where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("area", NullValueHandling = NullValueHandling.Ignore)]
        public string[] AreaIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the automation blueprints where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("blueprint", NullValueHandling = NullValueHandling.Ignore)]
        public string[] AutomationBlueprintIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the automations where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("automation", NullValueHandling = NullValueHandling.Ignore)]
        public string[] AutomationIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the configuration entries associated to the target entity of the search operation.
        /// </summary>
        [JsonProperty("config_entry", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ConfigEntryIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the devices related with the target entity of the search operation.
        /// </summary>
        [JsonProperty("device", NullValueHandling = NullValueHandling.Ignore)]
        public string[] DeviceIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the entities related with the target entity of the search operation.
        /// </summary>
        [JsonProperty("entity", NullValueHandling = NullValueHandling.Ignore)]
        public string[] EntityIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the floors where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("floor", NullValueHandling = NullValueHandling.Ignore)]
        public string[] FloorIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the groups where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public string[] GroupIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the helpers where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("helper", NullValueHandling = NullValueHandling.Ignore)]
        public string[] HelperIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the integrations where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("integration", NullValueHandling = NullValueHandling.Ignore)]
        public string[] IntegrationIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the labels where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string[] LabelIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the persons where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("person", NullValueHandling = NullValueHandling.Ignore)]
        public string[] PersonIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the scenes where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("scene", NullValueHandling = NullValueHandling.Ignore)]
        public string[] SceneIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the script blueprints where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("script_blueprint", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ScriptBlueprintIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The identifiers of the scripts where the target entity of the search operation is included.
        /// </summary>
        [JsonProperty("script", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ScriptIds { get; set; } = Array.Empty<string>();
    }
}
