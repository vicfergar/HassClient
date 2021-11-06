using HassClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// The Entity Registry keeps a registry of entities. Entities are uniquely identified by their domain, platform and
    /// an unique id provided by that platform.
    /// </summary>
    public class EntityRegistryEntry : EntityRegistryEntryBase
    {
        [JsonProperty]
        private readonly ModifiableProperty<DisabledByEnum?> disabledBy = new ModifiableProperty<DisabledByEnum?>(nameof(disabledBy));

        [JsonProperty(Required = Required.Always)]
        private string entityId;

        /// <inheritdoc />
        [JsonProperty]
        internal protected override string UniqueId { get; set; }

        /// <inheritdoc />
        public override string Name
        {
            get => base.Name ?? this.OriginalName;
            set => base.Name = value != this.OriginalName ? value : null;
        }

        /// <inheritdoc />
        public override string Icon
        {
            get => base.Icon ?? this.OriginalIcon;
            set => base.Icon = value != this.OriginalIcon ? value : null;
        }

        /// <inheritdoc />
        protected override bool AcceptsNullOrWhiteSpaceName => true;

        /// <inheritdoc />
        public override string EntityId => this.entityId;

        /// <summary>
        /// Gets the original friendly name of this entity.
        /// </summary>
        [JsonProperty]
        public string OriginalName { get; protected set; }

        /// <summary>
        /// Gets the original icon to display in front of the entity in the front-end.
        /// </summary>
        [JsonProperty]
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
        public DisabledByEnum DisabledBy => this.disabledBy.Value ?? DisabledByEnum.None;

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
        /// Gets a value indicating the classification for non-primary entities.
        /// <para>
        /// Primary entity's category will be <see cref="EntityCategory.None"/>.
        /// </para>
        /// </summary>
        [JsonProperty]
        public string EntityCategory { get; private set; }

        /// <summary>
        /// Gets the domain of the entity.
        /// </summary>
        [JsonIgnore]
        public string Domain => EntityIdHelpers.GetDomain(this.EntityId);

        [JsonConstructor]
        private EntityRegistryEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRegistryEntry"/> class.
        /// <para>
        /// Used for testing purposes.
        /// </para>
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="name">The original name.</param>
        /// <param name="icon">The original icon.</param>
        /// <param name="disabledBy">The original disable.</param>
        internal protected EntityRegistryEntry(string entityId, string name, string icon, DisabledByEnum disabledBy = DisabledByEnum.None)
            : base(name, icon)
        {
            this.entityId = entityId;
            this.Platform = entityId.GetDomain();
            this.disabledBy.Value = disabledBy;

            this.SaveChanges();
        }

        // Used for testing purposes.
        internal static EntityRegistryEntry CreateUnmodified(string entityId, string name, string icon = null, DisabledByEnum disabledBy = DisabledByEnum.None)
        {
            return new EntityRegistryEntry(entityId, name, icon, disabledBy);
        }

        // Used for testing purposes.
        internal static EntityRegistryEntry CreateFromEntry(EntityRegistryEntryBase entry, DisabledByEnum disabledBy = DisabledByEnum.None)
        {
            return new EntityRegistryEntry(entry.EntityId, entry.Name, entry.Icon, disabledBy);
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties().Append(this.disabledBy);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(EntityRegistryEntry)}: {this.EntityId}";

        // Used for testing purposes.
        internal EntityRegistryEntry Clone()
        {
            var result = CreateUnmodified(this.EntityId, this.Name, this.Icon, this.DisabledBy);
            result.UniqueId = this.UniqueId;
            result.entityId = this.entityId;
            result.AreaId = this.AreaId;
            result.Capabilities = this.Capabilities;
            result.ConfigEntryId = this.ConfigEntryId;
            result.DeviceClass = this.DeviceClass;
            result.DeviceId = this.DeviceId;
            result.OriginalName = this.OriginalName;
            result.OriginalIcon = this.OriginalIcon;
            result.Platform = this.Platform;
            result.SupportedFeatures = this.SupportedFeatures;
            result.UnitOfMeasurement = this.UnitOfMeasurement;
            return result;
        }
    }
}
