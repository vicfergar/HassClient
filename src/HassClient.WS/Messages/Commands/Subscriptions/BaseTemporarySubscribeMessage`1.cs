namespace HassClient.WS.Messages.Commands.Subscriptions
{
    /// <summary>
    /// Represents a message that creates a temporary subscription which auto-completes
    /// after the last expected event message is received.
    /// </summary>
    /// <typeparam name="TEventData">The type of the expected event data.</typeparam>
    public abstract class BaseTemporarySubscribeMessage<TEventData> : BaseOutgoingMessage, ISubscribeMessage
    {
        /// <inheritdoc/>
        public bool IsLongRunning => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTemporarySubscribeMessage{TEventData}"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseTemporarySubscribeMessage(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Determines if this is the last expected event message for the subscription.
        /// </summary>
        /// <param name="eventData">The incoming event data to check.</param>
        /// <returns><c>true</c> if this is the last expected event message; otherwise, <c>false</c>.</returns>
        public abstract bool IsLastEvent(TEventData eventData);
    }
}
