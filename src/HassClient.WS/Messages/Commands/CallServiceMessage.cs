using HassClient.Helpers;
using HassClient.Models;
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

        public CallServiceMessage(string domain, string service, object serviceData)
            : this()
        {
            this.Domain = domain;
            this.Service = service;
            this.ServiceData = serviceData;
        }

        public CallServiceMessage(KnownDomains domain, KnownServices service, object serviceData)
            : this(domain.ToDomainString(), service.ToServiceString(), serviceData)
        {
        }
    }
}
