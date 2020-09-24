using System;

namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Represents an authentication message used by Web Socket API.
    /// </summary>
    internal class AuthenticationRequiredMessage : BaseMessage
    {
        public Version HAVersion { get; set; }

        public AuthenticationRequiredMessage()
            : base("auth_required")
        {
        }
    }
}
