using System;
using System.Globalization;

namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a CIE 1931 XY coordinate pair.
    /// </summary>
    public class XYColor : Color
    {
        /// <summary>
        /// Gets the horizontal coordinate.
        /// </summary>
        public float X { get; internal set; }

        /// <summary>
        /// Gets the vertical coordinate.
        /// </summary>
        public float Y { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XYColor"/> class.
        /// </summary>
        /// <param name="x">The horizontal coordinate in the range [0, 1].</param>
        /// <param name="y">The vertical coordinate in the range [0, 1].</param>
        public XYColor(float x, float y)
            : base()
        {
            if (x < 0 || x > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(x), x, $"X value must be in the range [0.0, 1.0]");
            }

            if (y < 0 || y > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(y), y, $"Y value must be in the range [0.0, 1.0]");
            }

            this.X = x;
            this.Y = y;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            FormattableString line = $"[{this.X}, {this.Y}]";
            return line.ToString(CultureInfo.InvariantCulture);
        }
    }
}
