using System;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an active Web Socket subscription.
    /// </summary>
    public abstract class WSEventSubscription
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
        /// The last error encountered during the subscription.
        /// </summary>
        protected Exception LastError { get; private set; }

        /// <summary>
        /// Indicates whether the subscription is long-running.
        /// </summary>
        public bool IsLongRunning => (this.SubscribeMessage as ISubscribeMessage).IsLongRunning;

        /// <summary>
        /// Indicates whether the subscription is registered.
        /// </summary>
        public bool IsRegistered => this.Id > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="WSEventSubscription"/> class.
        /// </summary>
        /// <param name="subscribeMessage">The message used to subscribe to the event.</param>
        protected WSEventSubscription(BaseOutgoingMessage subscribeMessage)
        {
            if (subscribeMessage == null)
            {
                throw new ArgumentNullException(nameof(subscribeMessage));
            }

            this.SubscribeMessage = subscribeMessage;
        }

        /// <summary>
        /// Handles the event received from the Web Socket.
        /// </summary>
        /// <param name="eventMessage">The event message received from the Web Socket.</param>
        internal abstract void HandleEvent(IncomingEventMessage eventMessage);

        /// <summary>
        /// Handles the error encountered during the subscription.
        /// </summary>
        /// <param name="ex">The exception encountered during the subscription.</param>
        internal virtual void HandleError(Exception ex) => this.LastError = ex;
    }
}
