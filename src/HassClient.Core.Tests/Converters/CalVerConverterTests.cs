using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(CalVerConverter))]
    public class CalVerConverterTests
    {
        private readonly CalVerConverter converter = new CalVerConverter();

        private readonly CalVer testVersion = CalVer.Create("2022.02.4b3");

        [Test]
        public void CanConvertCalVer()
        {
            var canConvert = converter.CanConvert(typeof(CalVer));

            Assert.True(canConvert);
        }

        [Test]
        public void WriteJson()
        {
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            var serializer = JsonSerializer.Create();

            converter.WriteJson(jsonWriter, testVersion, serializer);

            Assert.AreEqual($"\"{testVersion}\"", textWriter.ToString());
        }

        [Test]
        public void ReadJson()
        {
            var textReader = new StringReader($"\"{testVersion}\"");
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = converter.ReadJson(jsonReader, testVersion.GetType(), null, serializer);

            Assert.NotNull(result);
            Assert.AreNotEqual(testVersion, result);
            Assert.AreEqual(testVersion.ToString(), result.ToString());
        }

        public void ReadJsonWithExistingValue()
        {
            var existingVersion = CalVer.Create("2021.05.7b1");

            var textReader = new StringReader(testVersion.ToString());
            var jsonReader = new JsonTextReader(textReader);
            var serializer = JsonSerializer.Create();
            var result = converter.ReadJson(jsonReader, testVersion.GetType(), existingVersion, serializer);

            Assert.NotNull(result);
            Assert.AreEqual(existingVersion, result);
            Assert.AreEqual(testVersion.ToString(), result.ToString());
        }
    }
}
