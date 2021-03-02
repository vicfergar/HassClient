using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a container for units of measure.
    /// </summary>
    public class UnitSystemModel
    {
        /// <summary>
        /// Gets the length unit (usually "km" or "mi").
        /// </summary>
        [JsonProperty]
        public string Length { get; private set; }

        /// <summary>
        /// Gets the mass unit (usually "g" or "lb").
        /// </summary>
        [JsonProperty]
        public string Mass { get; private set; }

        /// <summary>
        /// Gets the pressure unit (usually "Pa" or "psi").
        /// </summary>
        [JsonProperty]
        public string Pressure { get; private set; }

        /// <summary>
        /// Gets the temperature unit including degree symbol (usually "°C" or "°F").
        /// </summary>
        [JsonProperty]
        public string Temperature { get; private set; }

        /// <summary>
        /// Gets the volume unit (usually "L" or "gal").
        /// </summary>
        [JsonProperty]
        public string Volume { get; private set; }
    }
}
