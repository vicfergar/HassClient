using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// The result of a raw command operation.
    /// </summary>
    public class RawCommandResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result object of the command operation.
        /// </summary>
        public JRaw Result { get; set; }

        /// <summary>
        /// Gets or sets error information when the operation failed.
        /// </summary>
        public ErrorInfo Error { get; set; }

        internal static RawCommandResult FromResultMessage(ResultMessage message)
        {
            return new RawCommandResult()
            {
                Success = message.Success,
                Error = message.Error,
                Result = message.Result,
            };
        }
    }
}
