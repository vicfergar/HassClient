namespace HassClient.Core.Models
{
    /// <summary>
    /// Represents a Home Assistant color.
    /// </summary>
    public abstract class Color
    {
        /// <summary>
        /// Creates a <see cref="RGBColor"/> with the given values.
        /// </summary>
        /// <param name="red">The red color component value.</param>
        /// <param name="green">The green color component value.</param>
        /// <param name="blue">The blue color component value.</param>
        /// <returns>A <see cref="RGBColor"/> with the given values.</returns>
        public static RGBColor FromRGB(byte red, byte green, byte blue)
        {
            return new RGBColor(red, green, blue);
        }

        /// <summary>
        /// Creates a <see cref="HSColor"/> with the given values.
        /// </summary>
        /// <param name="hue">The hue value in the range [0, 360].</param>
        /// <param name="saturation">The saturation value in the range [0, 100].</param>
        /// <returns>A <see cref="HSColor"/> with the given values.</returns>
        public static HSColor FromHS(uint hue, uint saturation)
        {
            return new HSColor(hue, saturation);
        }

        /// <summary>
        /// Creates a <see cref="XYColor"/> with the given values.
        /// </summary>
        /// <param name="x">The horizontal coordinate in the range [0, 1].</param>
        /// <param name="y">The vertical coordinate in the range [0, 1].</param>
        /// <returns>A <see cref="XYColor"/> with the given values.</returns>
        public static XYColor FromXY(float x, float y)
        {
            return new XYColor(x, y);
        }

        /// <summary>
        /// Creates a <see cref="KelvinTemperatureColor"/> with the given temperature.
        /// </summary>
        /// <param name="kelvins">
        /// A value representing the color temperature in kelvins in the range [1000, 40000].
        /// </param>
        /// <returns>A <see cref="KelvinTemperatureColor"/> with the given temperature.</returns>
        public static KelvinTemperatureColor FromKelvinTemperature(uint kelvins)
        {
            return new KelvinTemperatureColor(kelvins);
        }

        /// <summary>
        /// Creates a <see cref="MiredsTemperatureColor"/> with the given temperature.
        /// </summary>
        /// <param name="mireds">
        /// A value representing the color temperature in mireds in the range [153, 500].
        /// </param>
        /// <returns>A <see cref="MiredsTemperatureColor"/> with the given temperature.</returns>
        public static MiredsTemperatureColor FromMireds(uint mireds)
        {
            return new MiredsTemperatureColor(mireds);
        }
    }
}
