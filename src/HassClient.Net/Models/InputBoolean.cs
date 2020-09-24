using Newtonsoft.Json;
using System;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    public class InputBoolean : RegistryEntryBase
    {
        /// <inheritdoc />
        [JsonProperty("id")]
        public override string UniqueId { get; internal set; }

        /// <summary>
        /// Gets or sets the initial value when Home Assistant starts.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Initial { get; set; }

        /// <inheritdoc />
        public override string EntityId => $"input_boolean.{this.UniqueId}";

        /// <inheritdoc />
        public override string ToString() => $"{nameof(InputBoolean)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is InputBoolean inputBoolean &&
                   this.UniqueId == inputBoolean.UniqueId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.UniqueId);
        }
    }
}
