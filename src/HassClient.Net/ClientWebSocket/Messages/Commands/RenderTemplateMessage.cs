using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HassClient.Net.WSMessages
{
    internal class RenderTemplateMessage : BaseOutgoingMessage
    {
        private TaskCompletionSource<string> templateEventReceivedTCS;

        [JsonIgnore]
        public Task<string> WaitResponseTask => this.templateEventReceivedTCS.Task;

        [JsonProperty(Required = Required.Always)]
        public string Template { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] EntitiesIds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Variables { get; set; }

        public RenderTemplateMessage()
            : base("render_template")
        {
            this.templateEventReceivedTCS = new TaskCompletionSource<string>();
        }

        public void ProcessEventReceivedMessage(EventResultMessage eventResultMessage)
        {
            var templateEventInfo = eventResultMessage.DeserializeEvent<TemplateEventInfo>();
            this.templateEventReceivedTCS.SetResult(templateEventInfo.Result);
        }
    }
}
