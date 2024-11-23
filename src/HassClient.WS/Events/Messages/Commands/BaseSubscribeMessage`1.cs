namespace HassClient.WS.Messages.Commands.Subscriptions
{
    /// <summary>
    /// Represents a message that creates a long-running subscription.
    /// </summary>
    /// <typeparam name="TEventData">The type of the expected event data.</typeparam>
    public abstract class BaseSubscribeMessage<TEventData> : BaseOutgoingMessage, ISubscribeMessage
    {
        /// <inheritdoc/>
        public bool IsLongRunning => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSubscribeMessage{TEventData}"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseSubscribeMessage(string type)
            : base(type)
        {
        }
    }
}
