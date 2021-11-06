using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines the source of an entity. This can be helpful to see if it's coming from configuration.yaml
    /// or a configuration entry, even if it has no unique ID.
    /// </summary>
    public class EntitySource
    {
        /// <summary>
        /// Gets the entity unique identifier.
        /// </summary>
        [JsonIgnore]
        public string EntityId { get; internal set; }

        /// <summary>
        /// Gets the entity domain.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the source from which the entity comes from. Usually <c>platform_config</c> or <c>config_entry</c>.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Source { get; private set; }

        /// <summary>
        /// Gets the configuration entry id associated with this entity.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigEntry { get; private set; }

        /// <summary>
        /// Gets a value indicathing whether the entity uses a custom component platform.
        /// </summary>
        [JsonProperty("custom_component")]
        public bool IsCustomComponent { get; private set; }

        /// <inheritdoc />
        public override string ToString() => $"Id: {this.EntityId} Domain: {this.Domain} Source: {this.Source} ";
    }
}
