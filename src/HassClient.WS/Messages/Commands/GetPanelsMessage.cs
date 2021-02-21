namespace HassClient.WS.Messages
{
    internal class GetPanelsMessage : BaseOutgoingMessage
    {
        public GetPanelsMessage()
            : base("get_panels")
        {
        }
    }
}
