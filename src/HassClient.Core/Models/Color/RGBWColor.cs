namespace HassClient.Models
{
    /// <summary>
    /// Represents an RGBW (red, green, blue, white) color.
    /// </summary>
    public class RGBWColor : RGBColor
    {
        /// <summary>
        /// Gets the white color component value.
        /// </summary>
        public byte W { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBWColor"/> class.
        /// </summary>
        /// <param name="red">The red color component value.</param>
        /// <param name="green">The green color component value.</param>
        /// <param name="blue">The blue color component value.</param>
        /// <param name="white">The white color component value.</param>
        public RGBWColor(byte red, byte green, byte blue, byte white)
            : base(red, green, blue)
        {
            this.W = white;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBWColor"/> class.
        /// </summary>
        /// <param name="color">A <see cref="System.Drawing.Color"/> color.</param>
        public RGBWColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        public static implicit operator RGBWColor(System.Drawing.Color x) => new RGBWColor(x);

        /// <inheritdoc/>
        public override string ToString() => $"[{this.R}, {this.G}, {this.B}, {this.W}]";
    }
}
