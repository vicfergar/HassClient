namespace HassClient.WS.Messages
{
    internal class EntitySourceMessage : BaseOutgoingMessage
    {
        public EntitySourceMessage()
            : base("entity/source")
        {
        }
    }
}
