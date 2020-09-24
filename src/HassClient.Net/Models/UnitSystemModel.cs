namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents a container for units of measure.
    /// </summary>
    public class UnitSystemModel
    {
        /// <summary>
        /// Gets or sets the length unit (usually "km" or "mi").
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// Gets or sets the mass unit (usually "g" or "lb").
        /// </summary>
        public string Mass { get; set; }

        /// <summary>
        /// Gets or sets the pressure unit (usually "Pa" or "psi").
        /// </summary>
        public string Pressure { get; set; }

        /// <summary>
        /// Gets or sets the temperature unit including degree symbol (usually "°C" or "°F").
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Gets or sets the volume unit (usually "L" or "gal").
        /// </summary>
        public string Volume { get; set; }
    }
}
