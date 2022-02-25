using System;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a temperature color expressed in kelvins.
    /// </summary>
    public class KelvinTemperatureColor : Color
    {
        /// <summary>
        /// Gets a value representing the color temperature in kelvins.
        /// </summary>
        public uint Kelvins { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KelvinTemperatureColor"/> class.
        /// </summary>
        /// <param name="kelvins">
        /// A value representing the color temperature in kelvins in the range [1000, 40000].
        /// </param>
        public KelvinTemperatureColor(uint kelvins)
        {
            this.Kelvins = Math.Min(Math.Max(kelvins, 1000), 40000);
        }

        /// <inheritdoc />
        public override string ToString() => this.Kelvins.ToString();
    }
}
