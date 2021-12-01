using HassClient.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HassClient.Serialization
{
    /// <summary>
    /// Converter for <see cref="Color"/>.
    /// </summary>
    public class ColorConverter : JsonConverter<Color>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        /// <inheritdoc />
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(RGBColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var rgbColor = existingValue as RGBColor;
                    rgbColor.R = (byte)values[0];
                    rgbColor.G = (byte)values[1];
                    rgbColor.B = (byte)values[1];
                    return rgbColor;
                }

                return Color.FromRGB((byte)values[0], (byte)values[1], (byte)values[2]);
            }
            else if (objectType == typeof(HSColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var hsColor = existingValue as HSColor;
                    hsColor.Hue = (uint)values[0];
                    hsColor.Saturation = (uint)values[1];
                    return hsColor;
                }

                return Color.FromHS((uint)values[0], (uint)values[1]);
            }
            else if (objectType == typeof(XYColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var xyColor = existingValue as XYColor;
                    xyColor.X = (float)values[0];
                    xyColor.Y = (float)values[1];
                    return xyColor;
                }

                return Color.FromXY((float)values[0], (float)values[1]);
            }
            else if (objectType == typeof(NameColor))
            {
                var colorName = serializer.Deserialize<string>(reader);
                if (hasExistingValue)
                {
                    var nameColor = existingValue as NameColor;
                    nameColor.Name = colorName;
                    return nameColor;
                }

                return new NameColor(colorName);
            }
            else if (objectType == typeof(MiredsTemperatureColor))
            {
                var mireds = serializer.Deserialize<uint>(reader);
                if (hasExistingValue)
                {
                    var color = existingValue as MiredsTemperatureColor;
                    color.Mireds = mireds;
                    return color;
                }

                return Color.FromMireds(mireds);
            }
            else if (objectType == typeof(KelvinTemperatureColor))
            {
                var kelvins = serializer.Deserialize<uint>(reader);
                if (hasExistingValue)
                {
                    var color = existingValue as KelvinTemperatureColor;
                    color.Kelvins = kelvins;
                    return color;
                }

                return Color.FromKelvinTemperature(kelvins);
            }

            return null;
        }
    }
}
