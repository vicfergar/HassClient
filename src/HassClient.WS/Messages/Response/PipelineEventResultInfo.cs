using HassClient.Helpers;
using HassClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Information of a fired Home Assistant pipeline event.
    /// </summary>
    public class PipelineEventResultInfo
    {
        /// <summary>
        /// Gets or sets the pipeline event type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        /// <summary>
        /// Gets the pipeline event type as <see cref="KnownPipelineEventTypes"/>.
        /// </summary>
        [JsonIgnore]
        public KnownPipelineEventTypes KnownType => this.Type.AsKnownPipelineEventType();

        /// <summary>
        /// Gets or sets the data associated with the fired event.
        /// </summary>
        [JsonProperty(Required = Required.AllowNull)]
        public JRaw Data { get; set; }

        /// <summary>
        /// Gets or sets the time at which the event was fired.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTimeOffset Timestamp { get; set; }
    }
}
