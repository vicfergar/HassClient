using HassClient.Models;
using HassClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents device trackers parameters used by <see cref="DeviceTrackerEntity.SeeAsync"/>.
    /// </summary>
    public class DeviceTrackerParams
    {
        [JsonProperty("dev_id", Required = Required.Always)]
        internal string deviceId;

        [JsonProperty(Required = Required.Always)]
        private string locationName => this.Location?.Name ?? KnownStates.NotHome.ToStateString();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private float[] gps => this.latitude.HasValue ?
            new[] { this.latitude.Value, this.longitude.Value } :
            null;

        /// <summary>
        /// Gets or sets the location as a <see cref="Zone"/> used for <c>location_name</c> attribute.
        /// <para>
        /// When <see langword="null"/>, <c>not_home</c> will be used.</para>
        /// </summary>
        [JsonIgnore]
        public Zone Location { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the device tracker.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HostName { get; set; }

        private string macAddress;

        /// <summary>
        /// Gets or sets the MAC address of the entity (only should be specified when updating a network based device tracker).
        /// </summary>
        [JsonProperty("mac", NullValueHandling = NullValueHandling.Ignore)]
        public string MacAddress
        {
            get => this.macAddress;
            set
            {
                value = value?.ToUpperInvariant();

                if (value != null &&
                    !Regex.IsMatch(value, @"^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$"))
                {
                    throw new FormatException($"{nameof(this.MacAddress)} has an invalid format.");
                }

                this.macAddress = value;
            }
        }

        private float? latitude;

        /// <summary>
        /// Gets or sets the latitude coordinate where device is located.
        /// </summary>
        [JsonIgnore]
        public float? Latitude
        {
            get => this.latitude;
            set => this.latitude = value.HasValue ? (float?)Math.Clamp(value.Value, -90, 90) : null;
        }

        private float? longitude;

        /// <summary>
        /// Gets or sets the longitude coordinate where device is located.
        /// </summary>
        [JsonIgnore]
        public float? Longitude
        {
            get => this.longitude;
            set => this.longitude = value.HasValue ? (float?)Math.Clamp(value.Value, -180, 180) : null;
        }

        /// <summary>
        /// Gets or sets the accuracy in meters of GPS coordinates.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? GPSAccuracy { get; set; }

        private float? batteryLevel;

        /// <summary>
        /// Gets or sets the percentage of battery level of the device tracker.
        /// </summary>
        [JsonProperty("battery", NullValueHandling = NullValueHandling.Ignore)]
        public float? BatteryLevel
        {
            get => this.batteryLevel;
            set => this.batteryLevel = value.HasValue ? (float?)Math.Clamp(value.Value, 0, 100) : null;
        }

        internal void CheckValues()
        {
            if (this.latitude.HasValue != this.longitude.HasValue)
            {
                throw new ArgumentException($"Both {this.Latitude} and {this.Location} properties or neither must be provided.");
            }
        }
    }
}
