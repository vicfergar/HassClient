using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a signle field in a service call.
    /// </summary>
    public class ServiceField
    {
        /// <summary>
        /// Gets the description of this field.
        /// </summary>
        [JsonProperty]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the example text for this field (may be <see langword="null"/>).
        /// </summary>
        [JsonProperty]
        public JRaw Example { get; private set; }
    }
}
