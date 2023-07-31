using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a response in a service call.
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// Gets a value indicating whether the response is optional.
        /// </summary>
        [JsonProperty("optional")]
        public string IsOptional { get; private set; }
    }
}
