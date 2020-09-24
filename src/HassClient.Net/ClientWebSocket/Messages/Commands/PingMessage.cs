namespace HassClient.Net.WSMessages
{
    internal class PingMessage : BaseOutgoingMessage
    {
        public PingMessage()
            : base("ping")
        {
        }
    }
}
