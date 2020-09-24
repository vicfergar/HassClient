namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents a basic message object returned from the server (e.g. an object with a "message" property).
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Gets or sets the message for this message object.
        /// </summary>
        public string Message { get; set; }
    }
}
