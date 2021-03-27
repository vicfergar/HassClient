using System;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a temperature color expressed in mireds.
    /// </summary>
    public class MiredsTemperatureColor : Color
    {
        /// <summary>
        /// Gets a value representing the color temperature in mireds.
        /// </summary>
        public uint Mireds { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MiredsTemperatureColor"/> class.
        /// </summary>
        /// <param name="mireds">
        /// A value representing the color temperature in mireds in the range [153, 500].
        /// </param>
        public MiredsTemperatureColor(uint mireds)
        {
            this.Mireds = Math.Clamp(mireds, 153, 500);
        }
    }
}
