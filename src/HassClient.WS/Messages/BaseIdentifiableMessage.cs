using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Represents an identifiable message used by Web Socket API.
    /// </summary>
    public abstract class BaseIdentifiableMessage : BaseMessage
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public uint Id { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIdentifiableMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseIdentifiableMessage(string type)
            : base(type)
        {
        }

        /// <inheritdoc />
        public override string ToString() => $"{base.ToString()} Id:{this.Id}";
    }
}
