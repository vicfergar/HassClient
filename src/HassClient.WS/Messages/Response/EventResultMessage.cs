using HassClient.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    internal class EventResultMessage : BaseIncomingMessage
    {
        public JRaw Event { get; set; }

        public EventResultMessage()
            : base("event")
        {
        }

        public T DeserializeEvent<T>()
        {
            return HassSerializer.DeserializeObject<T>(this.Event);
        }
    }
}
