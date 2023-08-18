using Newtonsoft.Json;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Defines information related with Home Assistant list of 'Assist Pipeline' definitions.
    /// </summary>
    public class PipelineList
    {
        /// <summary>
        /// Gets all defined 'Assist Pipelines' definitions.
        /// </summary>[SerializeField]
        [JsonProperty("pipelines")]
        public List<PipelineInfo> Pipelines { get; private set; }

        /// <summary>
        /// Gets the preferred defined Assist Pipeline.
        /// </summary>[SerializeField]
        [JsonProperty("preferred_pipeline")]
        public string PreferredPipeline;
    }
}
