using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    internal class ConfirmPushNotificationMessage : BaseOutgoingMessage
    {
        [JsonProperty]
        public string WebhookId { get; set; }

        [JsonProperty]
        public string ConfirmId { get; set; }

        public ConfirmPushNotificationMessage()
            : base("mobile_app/push_notification_confirm")
        {
        }
    }
}
