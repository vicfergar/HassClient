using HassClient.Helpers;
using HassClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HassClient.Serialization
{
    /// <summary>
    /// Helper class used by the Home Assistant client for serialization.
    /// </summary>
    public static class HassSerializer
    {
        private readonly static NamingStrategy namingStrategy = new SnakeCaseNamingStrategy { OverrideSpecifiedNames = false };

        internal static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = CreateContractResolver<DefaultContractResolver>(),
            Converters = new List<JsonConverter>
            {
                new ColorConverter(),
                new ModifiablePropertyConverter(),
                new StringEnumConverter(namingStrategy),
                new TupleSetToDictionaryConverter(),
            },
        };

        private readonly static JsonSerializer serializer = CreateSerializer();

        private static JsonSerializer CreateSerializer() => JsonSerializer.CreateDefault(DefaultSettings);

        private static T CreateContractResolver<T>()
            where T : DefaultContractResolver, new()
        {
            var result = new T();
            result.NamingStrategy = namingStrategy;
            return result;
        }

        /// <summary>
        /// Deserializes the <see cref="JRaw"/> object to the specified .NET type using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The <see cref="JRaw"/> object to deserialize.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized object from the <see cref="JRaw"/> object.</returns>
        public static T DeserializeObject<T>(JRaw value)
        {
            return value != null ? DeserializeObject<T>((string)value.Value) : default;
        }

        /// <summary>
        /// Deserializes the JSON string to the specified .NET type using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, DefaultSettings);
        }

        /// <summary>
        /// Deserializes the <see cref="JRaw"/> object to the specified .NET type using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The <see cref="JRaw"/> object to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <returns>The deserialized object from the <see cref="JRaw"/> object.</returns>
        public static object DeserializeObject(JRaw value, Type type)
        {
            return value != null ? DeserializeObject((string)value.Value, type) : default;
        }

        /// <summary>
        /// Deserializes the JSON string to the specified .NET type using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, DefaultSettings);
        }

        /// <summary>
        /// Populates the object with values from the <see cref="JRaw"/> object
        /// using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The <see cref="JRaw"/> object to deserialize.</param>
        /// <param name="target">The target object to populate values onto.</param>
        public static void PopulateObject(JRaw value, object target)
        {
            if (value != null)
            {
                JsonConvert.PopulateObject((string)value.Value, target, DefaultSettings);
            }
        }

        /// <summary>
        /// Populates the object with values from the JSON string using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <param name="target">The target object to populate values onto.</param>
        public static void PopulateObject(string value, object target)
        {
            JsonConvert.PopulateObject(value, target, DefaultSettings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, DefaultSettings);
        }

        /// <summary>
        /// Creates a <see cref="JObject"/> from an object.
        /// </summary>
        /// <param name="value">The object that will be used to create the <see cref="JObject"/>.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to be included in the <see cref="JObject"/>.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>A <see cref="JObject"/> with the values of the specified object.</returns>
        public static JObject CreateJObject(object value, IEnumerable<string> selectedProperties = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var ser = serializer;

            if (selectedProperties != null)
            {
                var contractResolver = CreateContractResolver<SelectedPropertiesContractResolver>();
                contractResolver.SelectedProperties = selectedProperties;
                ser = CreateSerializer();
                ser.ContractResolver = contractResolver;
            }

            return JObject.FromObject(value, ser);
        }

        /// <summary>
        /// Converts a <see cref="Enum"/> value to a snake case <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type of value.</typeparam>
        /// <param name="value">The <typeparamref name="TEnum"/> value to be converted.</param>
        /// <returns>The value name converted to snake case.</returns>
        public static string ToSnakeCase<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            if (value is KnownDomains ||
                value is KnownEventTypes ||
                value is KnownServices ||
                value is KnownStates)
            {
                throw new InvalidOperationException($"For known enums use {nameof(KnownEnumHelpers)} extension methods.");
            }

            return ToSnakeCaseUnchecked(value);
        }

        internal static string ToSnakeCaseUnchecked<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            return SerializeObject(value).Trim('"');
        }

        /// <summary>
        /// Tries to parse the <typeparamref name="TEnum"/> value from a snake case <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
        /// <param name="value">The snake case <see cref="string"/> representation of the enumeration name or underlying value to convert.</param>
        /// <param name="result">When this method returns, result contains an object of type <typeparamref name="TEnum"/> whose value
        /// is represented by value if the parse operation succeeds. If the parse operation fails, result contains the default value
        /// of the underlying type of <typeparamref name="TEnum"/>. Note that this value need not be a member of the
        /// <typeparamref name="TEnum"/> enumeration. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the value parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnumFromSnakeCase<TEnum>(string value, out TEnum result)
            where TEnum : Enum
        {
            try
            {
                result = DeserializeObject<TEnum>($"\"{value}\"");
                return true;
            }
            catch (JsonSerializationException)
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Gets a the name of the property once serialized using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="propertyInfo">An <see cref="PropertyInfo"/> object representing the property.</param>
        /// <returns>An <see cref="string"/> representing the name of the property once serialized using default
        /// <see cref="HassSerializer"/> settings.</returns>
        public static string GetSerializedPropertyName(PropertyInfo propertyInfo)
        {
            var attributePropertyName = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
            return attributePropertyName ?? GetDefaultSerializedPropertyName(propertyInfo.Name);
        }

        /// <summary>
        /// Gets a the name of the property once serialized using default <see cref="HassSerializer"/> settings.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>An <see cref="string"/> representing the name of the property once serialized using default
        /// <see cref="HassSerializer"/> settings.</returns>
        public static string GetDefaultSerializedPropertyName(string name)
        {
            return namingStrategy.GetPropertyName(name, false);
        }
    }
}
