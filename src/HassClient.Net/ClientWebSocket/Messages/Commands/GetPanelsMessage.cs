namespace HassClient.Net.WSMessages
{
    internal class GetPanelsMessage : BaseOutgoingMessage
    {
        public GetPanelsMessage()
            : base("get_panels")
        {
        }
    }
}
