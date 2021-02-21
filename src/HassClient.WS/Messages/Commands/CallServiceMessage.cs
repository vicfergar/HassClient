using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class CallServiceMessage : BaseOutgoingMessage
    {
        [JsonProperty(Required = Required.Always)]
        public string Domain { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Service { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object ServiceData { get; set; }

        public CallServiceMessage()
            : base("call_service")
        {
        }
    }
}
