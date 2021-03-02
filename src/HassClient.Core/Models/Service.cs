using Newtonsoft.Json;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a signle service definition.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets the description of the service object.
        /// </summary>
        [JsonProperty]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the fields/parameters that the service supports.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, ServiceField> Fields { get; private set; }
    }
}
