namespace HassClient.WS.Messages
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationInvalidMessage : BaseMessage
    {
        public string Message { get; set; }

        public AuthenticationInvalidMessage()
            : base("auth_invalid")
        {
        }
    }
}
