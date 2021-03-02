using HassClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HassClient.Models
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
        /// Gets the platform associated with this entity registry.
        /// </summary>
        [JsonProperty]
        public string Platform { get; private set; }

        /// <summary>
        /// Gets the device id associated with this entity registry.
        /// </summary>
        [JsonProperty]
        public string DeviceId { get; private set; }

        /// <summary>
        /// Gets the area id associated with this entity registry.
        /// </summary>
        [JsonProperty]
        public string AreaId { get; private set; }

        /// <summary>
        /// Gets the configuration entry id associated with this entity registry.
        /// </summary>
        [JsonProperty]
        public string ConfigEntryId { get; internal set; }

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
        /// Gets the capabilities of the entity.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, JRaw> Capabilities { get; private set; }

        /// <summary>
        /// Gets a the supported features of this entity, if any.
        /// </summary>
        [JsonProperty]
        public int SupportedFeatures { get; private set; }

        /// <summary>
        /// Gets the class of the device. This affects the state and default icon representation
        /// of the entity.
        /// </summary>
        [JsonProperty]
        public string DeviceClass { get; private set; }

        /// <summary>
        /// Gets the units of measurement, if any. This will also influence the graphical presentation
        /// in the history visualization as continuous value.
        /// Sensors with missing <see cref="UnitOfMeasurement"/> are showing as discrete values.
        /// </summary>
        [JsonProperty]
        public string UnitOfMeasurement { get; private set; }

        /// <summary>
        /// Gets the domain of the entity.
        /// </summary>
        [JsonIgnore]
        public string Domain => EntityIdHelpers.GetDomain(this.EntityId);

        // Needed for serialization.
        private RegistryEntry()
            : base(null, null)
        {
        }

        // Used for testing purposes.
        internal RegistryEntry(string entityId, string name, string icon, DisabledByEnum disabledBy = DisabledByEnum.None)
            : base(name, icon)
        {
            this.entityId = entityId;
            this.Platform = entityId.GetDomain();
            this.disabledBy = disabledBy;

            this.ClearPendingChanges();
        }

        // Used for testing purposes.
        internal static RegistryEntry CreateFromEntry(RegistryEntryBase entry, DisabledByEnum disabledBy = DisabledByEnum.None)
        {
            return new RegistryEntry(entry.EntityId, entry.Name, entry.Icon, disabledBy);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(RegistryEntry)}: {this.EntityId}";

        internal void Update(RegistryEntry updatedEntity, string newEntityId)
        {
            if (newEntityId != null)
            {
                this.entityId = newEntityId;
            }

            this.disabledBy = updatedEntity.disabledBy;

            this.Update(updatedEntity);
        }
    }
}
