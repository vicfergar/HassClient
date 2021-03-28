using System;

namespace HassClient.WS
{
    /// <summary>
    /// Represents the connection parameters used by <see cref="HassWSApi"/>.
    /// </summary>
    public class ConnectionParameters
    {
        private Uri endpoint;

        /// <summary>
        /// Gets or sets an <see cref="Uri"/> representing the web socket connection endpoint. (e.g. <c>ws://localhost:8123/api/websocket</c>).
        /// </summary>
        public Uri Endpoint
        {
            get => this.endpoint;
            set
            {
                if (value.Scheme != "ws" && value.Scheme != "wss")
                {
                    throw new ArgumentException($"Invalid URI Scheme: {value.Scheme}");
                }

                this.endpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the access token to be used during authentication phase.
        /// <para>
        /// You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser,
        /// and going to your profile <c>http://IP_ADDRESS:8123/profile</c>.
        /// </para>
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Creates connection parameters based on the specified Home Assistant base URL and Access Token.
        /// </summary>
        /// <param name="instanceBaseUrl">
        /// An URL representing the Home Assistant instance base address. (e.g. "http://localhost:8123").
        /// </param>
        /// <param name="accessToken">
        /// The access token to be used during authentication phase.
        /// <para>
        /// You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser,
        /// and going to your profile http://IP_ADDRESS:8123/profile.
        /// </para>
        /// </param>
        /// <returns>
        /// The connection parameters based on the specified Home Assistant base URL and Access Token.
        /// </returns>
        public static ConnectionParameters CreateFromInstanceBaseUrl(string instanceBaseUrl, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(instanceBaseUrl))
            {
                throw new ArgumentException($"'{nameof(instanceBaseUrl)}' cannot be null or whitespace", nameof(instanceBaseUrl));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException($"'{nameof(accessToken)}' cannot be null or whitespace", nameof(accessToken));
            }

            var uriBuilder = new UriBuilder(instanceBaseUrl);
            if (uriBuilder.Scheme == Uri.UriSchemeHttp)
            {
                uriBuilder.Scheme = "ws";
            }
            else if (uriBuilder.Scheme == Uri.UriSchemeHttps)
            {
                uriBuilder.Scheme = "wss";
            }
            else
            {
                throw new ArgumentException("Invalid URI Scheme", nameof(instanceBaseUrl));
            }

            uriBuilder.Path = "/api/websocket";

            return new ConnectionParameters()
            {
                Endpoint = uriBuilder.Uri,
                AccessToken = accessToken,
            };
        }

        /// <summary>
        /// Creates connection parameters based the add-ons internal proxy. This can only be used when connecting tithing a
        /// home assistant add-on.
        /// <para>
        /// More information at <see href="https://developers.home-assistant.io/docs/add-ons/communication#home-assistant-core"/>.
        /// </para>
        /// </summary>
        /// <returns>The connection parameters based the add-ons internal proxy.</returns>
        public static ConnectionParameters CreateForAddonConnection()
        {
            const string tokenEnvVar = "SUPERVISOR_TOKEN";
            var supervisorToken = Environment.GetEnvironmentVariable(tokenEnvVar);

            if (string.IsNullOrEmpty(supervisorToken))
            {
                throw new InvalidOperationException($"Error initializing API as Supervisor: Environment variable '{tokenEnvVar}' not found.");
            }

            return new ConnectionParameters()
            {
                Endpoint = new Uri("ws://supervisor/core/websocket"),
                AccessToken = supervisorToken,
            };
        }
    }
}
