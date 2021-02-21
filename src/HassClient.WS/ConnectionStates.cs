namespace HassClient.WS
{
    /// <summary>
    /// Defines the connection states for <see cref="HassClientWebSocket"/>.
    /// </summary>
    public enum ConnectionStates
    {
        /// <summary>
        /// The web socket client is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The web socket client is staring a connection.
        /// </summary>
        Connecting,

        /// <summary>
        /// The web socket client is in the authenticating phase.
        /// </summary>
        Authenticating,

        /// <summary>
        /// The web socket client is connected and listening for commands.
        /// </summary>
        Connected,
    }
}
