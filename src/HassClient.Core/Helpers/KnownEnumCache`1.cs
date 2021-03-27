using HassClient.Serialization;
using System;

namespace HassClient.Helpers
{
    /// <summary>
    /// Cache used to reduce use of string in KnownEnum types.
    /// </summary>
    /// <typeparam name="TEnum">The KnownEnum type.</typeparam>
    internal class KnownEnumCache<TEnum>
            where TEnum : struct, Enum
    {
        private Map<string, TEnum> cache = new Map<string, TEnum>();

        private TEnum? valueForNullString;

        public KnownEnumCache(TEnum? valueForNullString = null)
        {
            this.valueForNullString = valueForNullString;
        }

        public TEnum AsEnum(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (this.valueForNullString.HasValue)
                {
                    return this.valueForNullString.Value;
                }
                else
                {
                    throw new ArgumentException($"'{nameof(value)}' cannot be null or empty", nameof(value));
                }
            }

            if (!this.cache.Forward.TryGetValue(value, out var result) &&
                HassSerializer.TryGetEnumFromSnakeCase(value, out result))
            {
                this.cache.Add(value, result);
            }

            return result;
        }

        public string AsString(TEnum value)
        {
            if (!this.cache.Reverse.TryGetValue(value, out var result))
            {
                result = value.ToSnakeCaseUnchecked();
                this.cache.Add(result, value);
            }

            return result;
        }
    }
}
