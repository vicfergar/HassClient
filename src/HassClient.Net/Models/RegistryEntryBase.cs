using Newtonsoft.Json;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Base class that defines a registry entry.
    /// </summary>
    public abstract class RegistryEntryBase
    {
        /// <summary>
        /// Gets the entity identifier of the entity.
        /// </summary>
        [JsonIgnore]
        public abstract string EntityId { get; }

        /// <summary>
        /// Gets or sets the friendly name of this entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon to display in front of the entity in the front-end.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// Gets the unique identifier of this entity.
        /// </summary>
        public abstract string UniqueId { get; internal set; }
    }
}
