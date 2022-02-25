using HassClient.Models;
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
            if (value is NameColor)
            {
                serializer.Serialize(writer, value.ToString());
            }
            else if (value is KelvinTemperatureColor kelvinColor)
            {
                serializer.Serialize(writer, kelvinColor.Kelvins);
            }
            else if (value is MiredsTemperatureColor miredsColor)
            {
                serializer.Serialize(writer, miredsColor.Mireds);
            }
            else
            {
                serializer.Serialize(writer, JArray.Parse(value.ToString()));
            }
        }

        /// <inheritdoc />
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(RGBWColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var rgbwColor = existingValue as RGBWColor;
                    rgbwColor.R = (byte)values[0];
                    rgbwColor.G = (byte)values[1];
                    rgbwColor.B = (byte)values[2];
                    rgbwColor.W = (byte)values[3];
                    return rgbwColor;
                }

                return Color.FromRGBW((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);
            }
            else if (objectType == typeof(RGBWWColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var rgbwwColor = existingValue as RGBWWColor;
                    rgbwwColor.R = (byte)values[0];
                    rgbwwColor.G = (byte)values[1];
                    rgbwwColor.B = (byte)values[2];
                    rgbwwColor.CW = (byte)values[3];
                    rgbwwColor.WW = (byte)values[4];
                    return rgbwwColor;
                }

                return Color.FromRGBWW((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3], (byte)values[4]);
            }
            else if (objectType == typeof(RGBColor))
            {
                var values = serializer.Deserialize<JArray>(reader);
                if (hasExistingValue)
                {
                    var rgbColor = existingValue as RGBColor;
                    rgbColor.R = (byte)values[0];
                    rgbColor.G = (byte)values[1];
                    rgbColor.B = (byte)values[2];
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
