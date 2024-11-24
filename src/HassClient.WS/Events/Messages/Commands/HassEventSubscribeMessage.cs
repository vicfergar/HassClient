using HassClient.Models;
using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    internal class HassEventSubscribeMessage : BaseSubscribeMessage
    {
        /// <inheritdoc/>
        public override bool IsExclusive => false;

        [JsonProperty]
        public string EventType { get; set; }

        public HassEventSubscribeMessage()
            : base("subscribe_events")
        {
        }

        public HassEventSubscribeMessage(string eventType)
            : this()
        {
            this.EventType = eventType;
        }

        private bool ShouldSerializeEventType() => this.EventType != Event.AnyEventFilter && this.EventType != null;
    }
}
