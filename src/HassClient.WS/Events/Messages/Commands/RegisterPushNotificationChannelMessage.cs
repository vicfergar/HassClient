using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    internal class RegisterPushNotificationChannelMessage : BaseSubscribeMessage
    {
        /// <inheritdoc/>
        public override bool IsExclusive => true;

        [JsonProperty]
        public string WebhookId { get; set; }

        [JsonProperty]
        public bool SupportConfirm { get; set; }

        public RegisterPushNotificationChannelMessage()
            : base("mobile_app/push_notification_channel")
        {
        }
    }
}
