using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(ColorConverter))]
    public class ColorConverterTests
    {
        private readonly ColorConverter converter = new ColorConverter();

        [Test]
        [TestCase(typeof(Color))]
        [TestCase(typeof(HSColor))]
        [TestCase(typeof(KelvinTemperatureColor))]
        [TestCase(typeof(MiredsTemperatureColor))]
        [TestCase(typeof(NameColor))]
        [TestCase(typeof(RGBColor))]
        [TestCase(typeof(RGBWColor))]
        [TestCase(typeof(XYColor))]
        public void CanConvertColors(Type colorType)
        {
            var canConvert = converter.CanConvert(colorType);

            Assert.True(canConvert);
        }

        public static IEnumerable<TestCaseData> WriteReadJsonTestCases()
        {
            var createData = (Color color) => new TestCaseData(color).SetName($"{{m}}{color.GetType().Name}");

            yield return createData(new RGBColor(10, 20, 30));
            yield return createData(new RGBWColor(10, 20, 30, 255));
            yield return createData(new HSColor(10, 20));
            yield return createData(new XYColor(0.2f, 0.6f));
            yield return createData(new NameColor("test_color"));
            yield return createData(new KelvinTemperatureColor(1337));
            yield return createData(new MiredsTemperatureColor(256));
        }

        [Test]
        [TestCaseSource(nameof(WriteReadJsonTestCases))]
        public void WriteJson(Color color)
        {
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            var serializer = JsonSerializer.Create();
            converter.WriteJson(jsonWriter, color, serializer);

            Assert.AreEqual($"\"{color}\"", textWriter.ToString());
        }

        [Test]
        [TestCaseSource(nameof(WriteReadJsonTestCases))]
        public void ReadJson(Color color)
        {
            var textReader = new StringReader(GetJsonRepresentation(color));
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = converter.ReadJson(jsonReader, color.GetType(), null, serializer);

            Assert.NotNull(result);
            Assert.AreNotEqual(color, result);
            Assert.AreEqual(color.ToString(), result.ToString());
        }

        public static IEnumerable<TestCaseData> ReadJsonWithExisingValueTestCases()
        {
            var createData = (Color existing, Color color) => new TestCaseData(existing, color).SetName($"{{m}}{color.GetType().Name}");

            yield return createData(new RGBColor(10, 20, 30), new RGBColor(40, 50, 60));
            yield return createData(new RGBWColor(10, 20, 30, 255), new RGBWColor(40, 50, 60, 128));
            yield return createData(new HSColor(10, 20), new HSColor(30, 40));
            yield return createData(new XYColor(0.2f, 0.6f), new XYColor(0.4f, 0.8f));
            yield return createData(new NameColor("test_color"), new NameColor("new_color"));
            yield return createData(new KelvinTemperatureColor(1337), new KelvinTemperatureColor(2001));
            yield return createData(new MiredsTemperatureColor(256), new MiredsTemperatureColor(106));
        }

        [Test]
        [TestCaseSource(nameof(ReadJsonWithExisingValueTestCases))]
        public void ReadJsonWithExisingValue(Color existing, Color color)
        {
            var textReader = new StringReader(GetJsonRepresentation(color));
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = converter.ReadJson(jsonReader, color.GetType(), existing, serializer);

            Assert.NotNull(result);
            Assert.AreEqual(existing, result);
            Assert.AreEqual(color.ToString(), result.ToString());
        }

        private string GetJsonRepresentation(Color color)
        {
            if (color is NameColor)
            {
                return $"\"{color}\"";
            }
            else if (color is KelvinTemperatureColor kelvinColor)
            {
                return kelvinColor.Kelvins.ToString();
            }
            else if (color is MiredsTemperatureColor miredsColor)
            {
                return miredsColor.Mireds.ToString();
            }
            else
            {
                return color.ToString();
            }
        }
    }
}
