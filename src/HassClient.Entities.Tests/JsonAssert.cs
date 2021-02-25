using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace HassClient.Entities.Tests
{
    public static class JsonAssert
    {
        public static void IsValid(string actual)
        {
            Assert.DoesNotThrow(() => JObject.Parse(actual), $"'{actual}' is not a valid JSON value.");
        }

        public static void IsEmpty(string actual)
        {
            IsValid(actual);

            var actualJson = JObject.Parse(actual);
            Assert.IsFalse(actualJson.HasValues, "JSON is not empty.");
        }

        public static void IsSingleField(string actual)
        {
            IsValid(actual);

            var actualJson = JObject.Parse(actual);
            Assert.NotZero(actualJson.Count, "JSON has any field.");
            Assert.AreEqual(1, actualJson.Count, "JSON has more than one field.");
        }

        public static void IsSingleField<TValue>(string expectedFieldName, TValue expectedFieldValue, string actual)
        {
            IsSingleField(actual);
            HasField(expectedFieldName, expectedFieldValue, actual);
        }

        public static void HasFieldsCount(int count, string actual)
        {
            IsValid(actual);

            var actualJson = JObject.Parse(actual);
            Assert.AreEqual(count, actualJson.Count, $"JSON has more than {count} field.");
        }

        public static void HasField(string expectedFieldName, string actual)
        {
            var actualJson = JObject.Parse(actual);
            Assert.NotNull(actualJson[expectedFieldName], $"JSON doest not has the field '{expectedFieldName}'.");
        }

        public static void HasField<TValue>(string expectedFieldName, TValue expectedFieldValue, string actual)
        {
            var actualJson = JObject.Parse(actual);
            var field = actualJson[expectedFieldName];
            Assert.NotNull(field, $"JSON doest not has the field '{expectedFieldName}'.");

            TValue fieldValue = default;
            Assert.DoesNotThrow(() => fieldValue = field.Value<TValue>(), $"JSON field cannot be casted to {typeof(TValue).Name}. Field value: {field}");
            Assert.AreEqual(expectedFieldValue, fieldValue, $"JSON field has different value. Expected:'{expectedFieldName}' Actual:'{field.Path}'.");
        }

        public static void HasField<TValue>(string expectedFieldName, TValue[] expectedFieldValues, string actual)
        {
            var actualJson = JObject.Parse(actual);
            var field = actualJson[expectedFieldName];
            Assert.NotNull(field, $"JSON doest not has the field '{expectedFieldName}'.");
            Assert.AreEqual(JTokenType.Array, field.Type, "JSON field value is not an array.");

            var values = field.Values<TValue>();
            CollectionAssert.AreEqual(expectedFieldValues, values);
        }
    }
}
