using System;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents a Web Socket subscription with a callback.
    /// </summary>
    public class CallbackEventSubscription : WSEventSubscription
    {
        private readonly Action<IncomingEventMessage> callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackEventSubscription"/> class.
        /// </summary>
        /// <param name="subscribeMessage">The message used to subscribe to the event.</param>
        /// <param name="callback">The callback to be invoked when the event is received.</param>
        public CallbackEventSubscription(
            BaseOutgoingMessage subscribeMessage,
            Action<IncomingEventMessage> callback)
            : base(subscribeMessage)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <summary>
        /// Handles the event received from the Web Socket.
        /// </summary>
        /// <param name="eventMessage">The event message received from the Web Socket.</param>
        internal override void HandleEvent(IncomingEventMessage eventMessage)
        {
            this.callback(eventMessage);
        }
    }
}
