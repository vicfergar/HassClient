using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    /// <summary>
    /// Represents a message that removes a long-running subscription.
    /// </summary>
    public abstract class BaseUnsubscribeMessage : BaseOutgoingMessage
    {
        /// <summary>
        /// The ID of the subscription to remove.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public uint Subscription { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUnsubscribeMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseUnsubscribeMessage(string type)
            : base(type)
        {
        }
    }
}
