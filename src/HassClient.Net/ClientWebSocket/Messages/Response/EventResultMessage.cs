using HassClient.Net.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.WSMessages
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
