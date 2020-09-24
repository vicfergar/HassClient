namespace HassClient.Net.WSMessages
{
    internal class PongMessage : BaseIncomingMessage
    {
        public PongMessage()
            : base("pong")
        {
        }
    }
}
