using HassClient.Models;
using Newtonsoft.Json;
using System;

namespace HassClient.Serialization
{
    /// <summary>
    /// Converter for <see cref="CalVer"/>.
    /// </summary>
    public class CalVerConverter : JsonConverter<CalVer>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, CalVer value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        /// <inheritdoc />
        public override CalVer ReadJson(JsonReader reader, Type objectType, CalVer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var versionStr = serializer.Deserialize<string>(reader);
            return CalVer.Parse(versionStr);
        }
    }
}
