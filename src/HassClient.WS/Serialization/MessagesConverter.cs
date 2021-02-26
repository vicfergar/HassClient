using HassClient.WS.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HassClient.WS.Serialization
{
    internal class MessagesConverter : JsonConverter
    {
        private readonly Type baseMessageType = typeof(BaseMessage);

        private readonly Dictionary<string, Func<BaseMessage>> factoriesByType;

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return this.baseMessageType.IsAssignableFrom(objectType);
        }

        public MessagesConverter()
        {
            this.factoriesByType = Assembly.GetAssembly(this.baseMessageType)
                                 .GetTypes()
                                 .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(this.baseMessageType) && x.GetConstructor(Type.EmptyTypes) != null)
                                 .Select(x => Expression.Lambda<Func<BaseMessage>>(Expression.New(x)).Compile())
                                 .ToDictionary(x => x().Type);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var messageType = (string)obj["type"];

            BaseMessage message;
            if (this.factoriesByType.TryGetValue(messageType, out var factory))
            {
                message = factory();
                serializer.Populate(obj.CreateReader(), message);
            }
            else
            {
                var id = obj.GetValue("id").Value<uint>();
                obj.Remove("id");
                obj.Remove("type");
                message = new RawCommandMessage(messageType, obj) { Id = id };
            }

            return message;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
