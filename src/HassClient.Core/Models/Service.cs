using Newtonsoft.Json;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a single service definition.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets the name of the service object.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description of the service object.
        /// </summary>
        [JsonProperty]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the targets that the service supports.
        /// </summary>
        [JsonProperty("target")]
        public Dictionary<string, Newtonsoft.Json.Linq.JRaw> Targets { get; private set; }

        /// <summary>
        /// Gets the fields/parameters that the service supports.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, ServiceField> Fields { get; private set; }
    }
}
