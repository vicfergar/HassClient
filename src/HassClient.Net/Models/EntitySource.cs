using Newtonsoft.Json;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Defines the source of an entity. This can be helpful to see if it's coming from configuration.yaml
    /// or a configuration entry, even if it has no unique ID.
    /// </summary>
    public class EntitySource
    {
        /// <summary>
        /// Gets or sets the entity unique identifier.
        /// </summary>
        [JsonIgnore]
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity domain.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the source from which the entity comes from. Usually <c>platform_config</c> or <c>config_entry</c>.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the configuration entry id associated with this entity.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigEntry { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"Id: {this.EntityId} Domain: {this.Domain} Source: {this.Source} ";
    }
}
