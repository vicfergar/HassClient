using HassClient.Net.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HassClient.Net.Models
{
    /// <summary>
    /// The Entity Registry keeps a registry of entities. Entities are uniquely identified by their domain, platform and
    /// an unique id provided by that platform.
    /// </summary>
    public class RegistryEntry : RegistryEntryBase
    {
        [JsonProperty("disabled_by")]
        private DisabledByEnum? disabledBy;

        [JsonProperty(Required = Required.Always)]
        private string entityId;

        /// <inheritdoc />
        public override string UniqueId { get; internal set; }

        /// <inheritdoc />
        public override string EntityId => this.entityId;

        /// <summary>
        /// Gets the original friendly name of this entity.
        /// </summary>
        public string OriginalName { get; protected set; }

        /// <summary>
        /// Gets the original icon to display in front of the entity in the front-end.
        /// </summary>
        public string OriginalIcon { get; protected set; }

        /// <summary>
        /// Gets or sets the platform associated with this entity registry.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the device id associated with this entity registry.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the area id associated with this entity registry.
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// Gets or sets the configuration entry id associated with this entity registry.
        /// </summary>
        public string ConfigEntryId { get; set; }

        /// <summary>
        /// Gets a value indicating the disabling source, if any.
        /// </summary>
        [JsonIgnore]
        public DisabledByEnum DisabledBy => this.disabledBy ?? DisabledByEnum.None;

        /// <summary>
        /// Gets a value indicating whether the entity is disabled.
        /// </summary>
        [JsonIgnore]
        public bool IsDisabled => this.DisabledBy != DisabledByEnum.None;

        /// <summary>
        /// Gets or sets the capabilities of the entity.
        /// </summary>
        public Dictionary<string, JRaw> Capabilities { get; set; }

        /// <summary>
        /// Gets or sets a the supported features of this entity, if any.
        /// </summary>
        public int SupportedFeatures { get; set; }

        /// <summary>
        /// Gets or sets the class of the device. This affects the state and default icon representation
        /// of the entity.
        /// </summary>
        public string DeviceClass { get; set; }

        /// <summary>
        /// Gets the units of measurement, if any. This will also influence the graphical presentation
        /// in the history visualization as continuous value.
        /// Sensors with missing <see cref="UnitOfMeasurement"/> are showing as discrete values.
        /// </summary>
        public string UnitOfMeasurement { get; set; }

        /// <summary>
        /// Gets the domain of the entity.
        /// </summary>
        [JsonIgnore]
        public string Domain => HassHelpers.GetDomain(this.EntityId);

        // Needed for serialization.
        internal RegistryEntry()
        {
        }

        // Used for testing purposes.
        internal RegistryEntry(string entityId, DisabledByEnum disabledBy = DisabledByEnum.None)
            : this()
        {
            this.entityId = entityId;
            this.Platform = entityId.GetDomain();
            this.disabledBy = disabledBy;
        }

        // Used for testing purposes.
        internal static RegistryEntry CreateFromEntry(RegistryEntryBase entry, DisabledByEnum disabledBy = DisabledByEnum.None)
        {
            return new RegistryEntry(entry.EntityId, disabledBy)
            {
                OriginalName = entry.Name,
                OriginalIcon = entry.Icon,
            };
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(RegistryEntry)}: {this.EntityId}";

        internal void Update(RegistryEntry updatedEntity, string newEntityId)
        {
            if (newEntityId != null)
            {
                this.entityId = newEntityId;
            }

            this.Name = updatedEntity.Name;
            this.Icon = updatedEntity.Icon;
            this.AreaId = updatedEntity.AreaId;
            this.disabledBy = updatedEntity.disabledBy;
        }
    }
}
