using Newtonsoft.Json;
using System;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Gets or sets the ID of this area.
        /// </summary>
        [JsonProperty(PropertyName = "area_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this area.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Area)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Area area &&
                   this.Id == area.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
    }
}
