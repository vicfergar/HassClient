using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Serialization
{
    /// <summary>
    /// Converter to convert JSON array of [key, value] pairs to <see cref="IReadOnlyDictionary{TKey, TValue}"/>,
    /// grouping multiple values with the same key into arrays.
    /// </summary>
    public class TupleSetToDictionaryConverter : JsonConverter<IReadOnlyDictionary<string, string[]>>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, IReadOnlyDictionary<string, string[]> value, JsonSerializer serializer)
        {
            var array = value
                .SelectMany(kvp => kvp.Value.Select(v => new[] { kvp.Key, v }))
                .ToArray();

            serializer.Serialize(writer, array);
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string[]> ReadJson(JsonReader reader, Type objectType, IReadOnlyDictionary<string, string[]> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = serializer.Deserialize<string[][]>(reader);

            if (array == null ||
                array.Length == 0)
            {
                return new Dictionary<string, string[]>();
            }

            return array.GroupBy(x => x[0])
                       .ToDictionary(
                           g => g.Key,
                           g => g.Select(x => x.Length > 1 ? x[1] : null)
                                .Where(x => x != null)
                                 .ToArray());
        }
    }
}
