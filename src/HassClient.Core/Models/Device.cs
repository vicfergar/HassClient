using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a device.
    /// <para>
    /// More information at <see href="https://developers.home-assistant.io/docs/device_registry_index/"/>.
    /// </para>
    /// </summary>
    public class Device : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> areaId = new ModifiableProperty<string>(nameof(AreaId));

        [JsonProperty]
        private readonly ModifiableProperty<DisabledByEnum?> disabledBy = new ModifiableProperty<DisabledByEnum?>(nameof(disabledBy));

        [JsonProperty]
        private readonly ModifiableProperty<string> nameByUser = new ModifiableProperty<string>(nameof(nameByUser));

        [JsonProperty("name")]
        private string originalName;

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the ID of this device.
        /// </summary>
        [JsonProperty]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the original name of the device assigned when was created.
        /// </summary>
        public string OriginalName => this.originalName;

        /// <summary>
        /// Gets the current name of this device.
        /// It will the one given by the user after creation; otherwise, <see cref="OriginalName"/>.
        /// <para>
        /// If set to <see langword="null"/>, the <see cref="OriginalName"/> will be used.
        /// </para>
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => this.nameByUser.Value ?? this.originalName;
            set => this.nameByUser.Value = value == this.originalName ? null : value;
        }

        /// <summary>
        /// Gets the unique ids of the configuration entries associated with this device.
        /// </summary>
        [JsonProperty("config_entries")]
        public string[] ConfigurationEntries { get; private set; }

        /// <summary>
        /// Gets a set of tuples of (connection_type, connection identifier).
        /// Connection types are defined in the device registry module.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Connections { get; private set; }

        /// <summary>
        /// Gets a set of identifiers. They identify the device in the outside world.
        /// An example is a serial number.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Identifiers { get; private set; }

        /// <summary>
        /// Gets the manufacturer of the device.
        /// </summary>
        [JsonProperty]
        public string Manufacturer { get; private set; }

        /// <summary>
        /// Gets the model of the device.
        /// </summary>
        [JsonProperty]
        public string Model { get; private set; }

        /// <summary>
        /// Gets the firmware version of the device.
        /// </summary>
        [JsonProperty]
        public string SWVersion { get; private set; }

        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DeviceEntryTypes EntryType { get; private set; }

        /// <summary>
        /// Gets the identifier of a device that routes messages between this device and Home Assistant.
        /// Examples of such devices are hubs, or parent devices of a sub-device.
        /// This is used to show device topology in Home Assistant.
        /// </summary>
        [JsonProperty]
        public string ViaDeviceId { get; private set; }

        /// <summary>
        /// Gets the area id which the device is placed in.
        /// </summary>
        public string AreaId
        {
            get => this.areaId.Value;
            set => this.areaId.Value = value;
        }

        /// <summary>
        /// Gets the suggested name for the area where the device is located.
        /// </summary>
        [JsonProperty]
        public string SuggestedArea { get; private set; }

        /// <summary>
        /// Gets a value indicating the disabling source, if any.
        /// </summary>
        [JsonIgnore]
        public DisabledByEnum DisabledBy => this.disabledBy.Value ?? DisabledByEnum.None;

        /// <summary>
        /// Gets a value indicating whether the device is disabled.
        /// </summary>
        [JsonIgnore]
        public bool IsDisabled => this.DisabledBy != DisabledByEnum.None;

        [JsonConstructor]
        private Device()
        {
        }

        // Used for testing purposes.
        internal static Device CreateUnmodified(string id, string name, string areaId = null, DisabledByEnum disabledBy = DisabledByEnum.None)
        {
            var result = new Device()
            {
                Id = id,
                originalName = name,
                AreaId = areaId,
            };

            result.disabledBy.Value = disabledBy;
            result.SaveChanges();

            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return this.areaId;
            yield return this.disabledBy;
            yield return this.nameByUser;
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Device)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Device device &&
                   this.Id == device.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
    }
}
