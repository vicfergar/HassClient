using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    internal class RenderTemplateMessage : BaseTemporarySubscribeMessage<TemplateEventInfo>
    {
        [JsonProperty(Required = Required.Always)]
        public string Template { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] EntitiesIds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Variables { get; set; }

        public RenderTemplateMessage()
            : base("render_template")
        {
        }

        public override bool IsLastEvent(TemplateEventInfo eventData)
        {
            // Template rendering is a one-time event
            return true;
        }
    }
}
