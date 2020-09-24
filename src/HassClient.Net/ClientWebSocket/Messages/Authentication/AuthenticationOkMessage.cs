using System;

namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationOkMessage : BaseMessage
    {
        public Version HAVersion { get; set; }

        public AuthenticationOkMessage()
            : base("auth_ok")
        {
        }
    }
}
