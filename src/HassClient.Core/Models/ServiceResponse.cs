using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents the service call response configuration.
    /// </summary>
    public class ServiceResponse
    {
         /// <summary>
        /// Gets a value indicating whether the response is optional.
        /// When true, the service optionally returns response data when asked by the caller.
        /// When false and the service has a response field, the service is read-only and must always return response data.
        /// When false and the service has no response field, the service does not support responses.
        /// </summary>
        [JsonProperty]
        public bool Optional { get; private set; }
    }
}
