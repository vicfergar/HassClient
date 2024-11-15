using System;

namespace HassClient.WS
{
    /// <summary>
    /// Represents a base class for API groups.
    /// </summary>
    public abstract class ResourceApi
    {
        /// <summary>
        /// The WebSocket client used to communicate with the Home Assistant instance.
        /// </summary>
        protected readonly HassClientWebSocket HassClientWebSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceApi"/> class.
        /// </summary>
        /// <param name="hassClientWebSocket">The WebSocket client used to communicate with the Home Assistant instance.</param>
        protected ResourceApi(HassClientWebSocket hassClientWebSocket)
        {
            this.HassClientWebSocket = hassClientWebSocket ?? throw new ArgumentNullException(nameof(hassClientWebSocket));
        }
    }
}
