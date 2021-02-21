namespace HassClient.WS.Messages
{
    internal class PongMessage : BaseIncomingMessage
    {
        public PongMessage()
            : base("pong")
        {
        }
    }
}
