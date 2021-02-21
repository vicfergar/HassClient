namespace HassClient.WS.Messages
{
    internal class GetServicesMessage : BaseOutgoingMessage
    {
        public GetServicesMessage()
            : base("get_services")
        {
        }
    }
}
