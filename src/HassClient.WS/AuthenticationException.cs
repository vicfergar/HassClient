using System;

namespace HassClient.WS
{
    /// <summary>
    /// Exception raised during Home Assistant Web Socket connection when authentication
    /// fails due to an invalid access token or an invalid message received from the server.
    /// </summary>
    public class AuthenticationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public AuthenticationException(string message)
            : base(message)
        {
        }
    }
}
