using System;
using HassClient.WS.Messages;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HassClient.WS
{
    /// <summary>
    /// Represents a Web Socket subscription with an asynchronous event channel.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    public class AsyncEventSubscription<TEventData> : WSEventSubscription
    {
        private readonly Channel<TEventData> eventChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEventSubscription{TEventData}"/> class.
        /// </summary>
        /// <param name="subscribeMessage">The message used to subscribe to the event.</param>
        public AsyncEventSubscription(BaseOutgoingMessage subscribeMessage)
            : base(subscribeMessage)
        {
            this.eventChannel = Channel.CreateUnbounded<TEventData>();
        }

        /// <summary>
        /// Handles the event received from the Web Socket.
        /// </summary>
        /// <param name="eventMessage">The event message received from the Web Socket.</param>
        internal override void HandleEvent(IncomingEventMessage eventMessage)
        {
            var eventData = eventMessage.DeserializeEvent<TEventData>();
            this.eventChannel.Writer.TryWrite(eventData);
        }

        /// <summary>
        /// Handles the error encountered during the subscription.
        /// </summary>
        /// <param name="ex">The exception encountered during the subscription.</param>
        internal override void HandleError(Exception ex)
        {
            base.HandleError(ex);
            this.eventChannel.Writer.Complete(ex);
        }

        /// <summary>
        /// Waits for the next event to be received asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The next event data received from the Web Socket.</returns>
        public async Task<TEventData> WaitForNextEventAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventChannel.Reader.ReadAsync(cancellationToken);
            }
            catch (ChannelClosedException) when (this.LastError != null)
            {
                throw this.LastError;
            }
        }

        /// <summary>
        /// Completes the subscription.
        /// </summary>
        public void Complete()
        {
            this.eventChannel.Writer.Complete();
        }
    }
}
