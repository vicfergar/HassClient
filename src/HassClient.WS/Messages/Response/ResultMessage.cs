using HassClient.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    internal class ResultMessage : BaseIncomingMessage
    {
        [JsonProperty(Required = Required.Always)]
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JRaw Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ErrorInfo Error { get; set; }

        public ResultMessage()
            : base("result")
        {
        }

        public T DeserializeResult<T>()
        {
            return HassSerializer.DeserializeObject<T>(this.Result);
        }
    }
}
