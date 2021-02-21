namespace HassClient.WS.Messages
{
    internal class PingMessage : BaseOutgoingMessage
    {
        public PingMessage()
            : base("ping")
        {
        }
    }
}
