namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Represents the error codes for <see cref="ErrorInfo"/>.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Undefined error code.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// A non-increasing identifier has been supplied.
        /// </summary>
        IdReuse = 1,

        /// <summary>
        /// Received message is not in expected format (voluptuous validation error).
        /// </summary>
        InvalidFormat = 2,

        /// <summary>
        /// Requested item cannot be found.
        /// </summary>
        NotFound = 3,

        /// <summary>
        /// Action not supported.
        /// </summary>
        NotSupported,

        /// <summary>
        /// General Home Assistant exception occurred.
        /// </summary>
        HomeAssistantError,

        /// <summary>
        /// The command is not recognized by Home Assistant.
        /// </summary>
        UnknownCommand,

        /// <summary>
        /// Unexpected error occurred.
        /// </summary>
        UnknownError,

        /// <summary>
        /// When an action is unauthorized.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Action exceeded the timeout period.
        /// </summary>
        Timeout,
    }
}
