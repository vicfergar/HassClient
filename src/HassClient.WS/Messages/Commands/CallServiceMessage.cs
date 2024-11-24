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

        [JsonProperty]
        public bool ReturnResponse { get; set; }

        public CallServiceMessage()
            : base("call_service")
        {
        }

        public CallServiceMessage(string domain, string service, object serviceData)
            : this()
        {
            this.Domain = domain;
            this.Service = service;
            this.ServiceData = serviceData;
        }
    }
}
