using System.Collections.Generic;
using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents filter conditions for a service field.
    /// </summary>
    public class ServiceFieldFilter
    {
        /// <summary>
        /// Gets the required supported features for this field.
        /// </summary>
        [JsonProperty]
        public List<int> SupportedFeatures { get; private set; }

        /// <summary>
        /// Gets the required attributes for this field.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, object> Attribute { get; private set; }
    }
}
