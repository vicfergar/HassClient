using HassClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    internal class UserResponse
    {
        [JsonProperty("user")]
        public JRaw UserRaw { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{this.UserRaw}";
    }
}
