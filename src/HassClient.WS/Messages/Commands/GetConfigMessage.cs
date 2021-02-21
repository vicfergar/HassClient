namespace HassClient.WS.Messages
{
    internal class GetConfigMessage : BaseOutgoingMessage
    {
        public GetConfigMessage()
            : base("get_config")
        {
        }
    }
}
