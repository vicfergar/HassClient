using System;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an active Web Socket subscription.
    /// </summary>
    public class WSEventSubscription
    {
        /// <summary>
        /// The ID of the subscription.
        /// </summary>
        public uint Id { get; internal set; }

        /// <summary>
        /// The message used to subscribe to the event.
        /// </summary>
        public BaseOutgoingMessage SubscribeMessage { get; private set; }

        /// <summary>
        /// The callback to be invoked when the event is received.
        /// </summary>
        public Action<IncomingEventMessage> Callback { get; private set; }

        /// <summary>
        /// Indicates whether the subscription is long-running.
        /// </summary>
        public bool IsLongRunning => (this.SubscribeMessage as ISubscribeMessage).IsLongRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="WSEventSubscription"/> class.
        /// </summary>
        /// <param name="subscribeMessage">The message used to subscribe to the event.</param>
        /// <param name="callback">The callback to be invoked when the event is received.</param>
        public WSEventSubscription(BaseOutgoingMessage subscribeMessage, Action<IncomingEventMessage> callback)
        {
            if (subscribeMessage == null)
            {
                throw new ArgumentNullException(nameof(subscribeMessage));
            }

            if (subscribeMessage.Id <= 0)
            {
                throw new ArgumentException("The message ID must be set", nameof(subscribeMessage));
            }

            this.Id = subscribeMessage.Id;
            this.SubscribeMessage = subscribeMessage;
            this.Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }
    }
}
