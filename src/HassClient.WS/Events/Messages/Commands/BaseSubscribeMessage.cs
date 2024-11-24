using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    /// <summary>
    /// Represents a message that creates a long-running subscription.
    /// </summary>
    public abstract class BaseSubscribeMessage : BaseOutgoingMessage, ISubscribeMessage
    {
        /// <inheritdoc/>
        public bool IsLongRunning => true;

        /// <summary>
        /// Indicates if this subscription should be exclusive, replacing any existing subscription of the same message type.
        /// </summary>
        [JsonIgnore]
        public abstract bool IsExclusive { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSubscribeMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseSubscribeMessage(string type)
            : base(type)
        {
        }
    }
}
