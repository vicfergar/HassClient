using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Base class representing Home Assistant WS messages.
    /// </summary>
    public abstract class BaseMessage
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessage"/> class.
        /// </summary>
        /// <param name="type">
        /// The message type as specified by Home Assistant WS API.
        /// <para>
        /// Due to lack of documentation in WS API, most message types are extracted
        /// from: <see href="https://github.com/home-assistant/core/search?q=async_register_command"/>.
        /// </para>
        /// </param>
        public BaseMessage(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new System.ArgumentException($"'{nameof(type)}' cannot be null or whitespace", nameof(type));
            }

            this.Type = type;
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Type}";
    }
}
