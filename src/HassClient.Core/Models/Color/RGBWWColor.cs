namespace HassClient.Models
{
    /// <summary>
    /// Represents an RGBWW (red, green, blue, cold white, warm white) color.
    /// </summary>
    public class RGBWWColor : RGBColor
    {
        /// <summary>
        /// Gets the cold white color component value.
        /// </summary>
        public byte CW { get; internal set; }

        /// <summary>
        /// Gets the warm white color component value.
        /// </summary>
        public byte WW { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBWWColor"/> class.
        /// </summary>
        /// <param name="red">The red color component value.</param>
        /// <param name="green">The green color component value.</param>
        /// <param name="blue">The blue color component value.</param>
        /// <param name="coldWhite">The cold white color component value.</param>
        /// <param name="warmWhite">The warm white color component value.</param>
        public RGBWWColor(byte red, byte green, byte blue, byte coldWhite, byte warmWhite)
            : base(red, green, blue)
        {
            this.CW = coldWhite;
            this.WW = warmWhite;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBWWColor"/> class.
        /// </summary>
        /// <param name="color">A <see cref="System.Drawing.Color"/> color.</param>
        public RGBWWColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A, color.A)
        {
        }

        public static implicit operator RGBWWColor(System.Drawing.Color x) => new RGBWWColor(x);

        /// <inheritdoc/>
        public override string ToString() => $"[{this.R}, {this.G}, {this.B}, {this.CW}, {this.WW}]";
    }
}
