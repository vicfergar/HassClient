using Newtonsoft.Json;
using System;

namespace HassClient.Converters
{
    /// <summary>
    /// Converts Unix timestamps to and from <see cref="DateTimeOffset"/> values.
    /// This converter handles both integer and floating-point Unix timestamps,
    /// preserving sub-second precision when present.
    /// </summary>
    /// <remarks>
    /// Unix timestamps represent the number of seconds that have elapsed since
    /// the Unix epoch (00:00:00 UTC on 1 January 1970).
    /// This implementation:
    /// - Handles null values by returning DateTimeOffset.MinValue.
    /// - Preserves fractional seconds during conversion.
    /// - Maintains UTC timezone information.
    /// </remarks>
    public class UnixTimestampConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Date)
            {
                return reader.Value is DateTime dt
                    ? new DateTimeOffset(dt)
                    : (DateTimeOffset)reader.Value;
            }

            if (reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer)
            {
                throw new JsonSerializationException($"Unexpected token type {reader.TokenType}. Expected numeric Unix timestamp.");
            }

            if (reader.Value == null)
            {
                return DateTimeOffset.MinValue;
            }

            double unixTimestamp = Convert.ToDouble(reader.Value);
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp)
                .AddSeconds(unixTimestamp % 1); // Handles fractional seconds
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            double unixTimestamp = value.ToUnixTimeMilliseconds() / 1000.0;
            writer.WriteValue(unixTimestamp);
        }
    }
}
