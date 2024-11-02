using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a single field in a service call.
    /// </summary>
    public class ServiceField
    {
        /// <summary>
        /// Gets the name of this field.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description of this field.
        /// </summary>
        [JsonProperty]
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the field is required or optional.
        /// </summary>
        [JsonProperty("required")]
        public string IsRequired { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the field is considered for advanced users.
        /// </summary>
        [JsonProperty("advanced")]
        public string IsAdvanced { get; private set; }

        /// <summary>
        /// Gets the default value of this field if defined; otherwise, <see langword="null"/>.
        /// </summary>
        [JsonProperty("default")]
        public JRaw DefaultValue { get; private set; }

        /// <summary>
        /// Gets the example for this field (may be <see langword="null"/>).
        /// </summary>
        [JsonProperty]
        public JRaw Example { get; private set; }

        /// <summary>
        /// Gets the selector for this field (may be <see langword="null"/>).
        /// </summary>
        [JsonProperty]
        public JRaw Selector { get; private set; }
    }
}
