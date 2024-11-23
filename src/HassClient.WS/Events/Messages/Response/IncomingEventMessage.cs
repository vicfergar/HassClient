using HassClient.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Represents an incoming event message.
    /// </summary>
    public class IncomingEventMessage : BaseIncomingMessage
    {
        /// <summary>
        /// The event data.
        /// </summary>
        public JRaw Event { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingEventMessage"/> class.
        /// </summary>
        public IncomingEventMessage()
            : base("event")
        {
        }

        /// <summary>
        /// Deserializes the event data to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the event data to.</typeparam>
        /// <returns>The deserialized event data.</returns>
        public T DeserializeEvent<T>()
        {
            return HassSerializer.DeserializeObject<T>(this.Event);
        }
    }
}
