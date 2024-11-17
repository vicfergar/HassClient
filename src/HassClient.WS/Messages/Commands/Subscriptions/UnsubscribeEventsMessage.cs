namespace HassClient.WS.Messages.Commands.Subscriptions
{
    internal class UnsubscribeEventsMessage : BaseUnsubscribeMessage
    {
        public UnsubscribeEventsMessage()
            : base("unsubscribe_events")
        {
        }
    }
}
