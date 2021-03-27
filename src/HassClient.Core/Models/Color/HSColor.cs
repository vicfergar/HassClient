using System;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a HSV color by hue and saturation.
    /// </summary>
    public class HSColor : Color
    {
        /// <summary>
        /// Gets the hue value.
        /// </summary>
        public uint Hue { get; internal set; }

        /// <summary>
        /// Gets the saturation value.
        /// </summary>
        public uint Saturation { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSColor"/> class.
        /// </summary>
        /// <param name="hue">The hue value in the range [0, 360].</param>
        /// <param name="saturation">The saturation value in the range [0, 100].</param>
        public HSColor(uint hue, uint saturation)
            : base()
        {
            if (hue > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(hue), hue, $"Hue value must be in the range [0, 360]");
            }

            if (saturation > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(saturation), saturation, $"Saturation value must be in the range [0, 100]");
            }

            this.Hue = hue;
            this.Saturation = saturation;
        }

        /// <inheritdoc />
        public override string ToString() => $"[{this.Hue}, {this.Saturation}]";
    }
}
