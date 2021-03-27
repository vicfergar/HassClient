using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a zone.
    /// </summary>
    [StorageEntityDomain(KnownDomains.Zone)]
    public class Zone : StorageEntityRegistryEntryBase
    {
        private readonly ModifiableProperty<float> latitude = new ModifiableProperty<float>(nameof(Latitude));

        private readonly ModifiableProperty<float> longitude = new ModifiableProperty<float>(nameof(Longitude));

        private readonly ModifiableProperty<bool> passive = new ModifiableProperty<bool>(nameof(IsPassive));

        private readonly ModifiableProperty<float> radius = new ModifiableProperty<float>(nameof(Radius));

        /// <summary>
        /// Gets or sets the latitude of the center point of the zone.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Latitude
        {
            get => this.latitude.Value;
            set => this.latitude.Value = value;
        }

        /// <summary>
        /// Gets or sets the longitude of the center point of the zone.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Longitude
        {
            get => this.longitude.Value;
            set => this.longitude.Value = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zone will be used only for automation and hide it
        /// from the frontend and not use the zone for device tracker name.
        /// </summary>
        [JsonProperty("passive", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPassive
        {
            get => this.passive.Value;
            set => this.passive.Value = value;
        }

        /// <summary>
        /// Gets or sets the radius of the zone in meters.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Radius
        {
            get => this.radius.Value;
            set => this.radius.Value = value;
        }

        [JsonConstructor]
        private Zone()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="longitude">The longitude of the center point of the zone.</param>
        /// <param name="latitude">The latitude of the center point of the zone.</param>
        /// <param name="radius">the radius of the zone in meters.</param>
        /// <param name="icon">The entity icon.</param>
        /// <param name="isPassive">
        /// Whether the zone will be used only for automation and hide it
        /// from the frontend and not use the zone for device tracker name.
        /// </param>
        public Zone(string name, float longitude, float latitude, float radius, string icon = null, bool isPassive = false)
            : base(name, icon)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Radius = radius;
            this.IsPassive = isPassive;
        }

        // Used for testing purposes.
        internal static Zone CreateUnmodified(string uniqueId, string name, float longitude, float latitude, float radius, string icon = null, bool isPassive = false)
        {
            var result = new Zone(name, longitude, latitude, radius, icon, isPassive) { Id = uniqueId };
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties()
                       .Append(this.latitude)
                       .Append(this.longitude)
                       .Append(this.passive)
                       .Append(this.radius);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Zone)}: {this.Name}";

        // Used for testing purposes.
        internal Zone Clone()
        {
            var result = CreateUnmodified(this.UniqueId, this.Name, this.Longitude, this.Latitude, this.Radius, this.Icon, this.IsPassive);
            return result;
        }
    }
}
