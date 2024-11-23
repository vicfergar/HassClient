namespace HassClient.WS.Messages
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationOkMessage : BaseMessage
    {
        public string HAVersion { get; set; }

        public AuthenticationOkMessage()
            : base("auth_ok")
        {
        }
    }
}
