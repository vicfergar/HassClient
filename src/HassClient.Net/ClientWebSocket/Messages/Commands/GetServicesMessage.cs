namespace HassClient.Net.WSMessages
{
    internal class GetServicesMessage : BaseOutgoingMessage
    {
        public GetServicesMessage()
            : base("get_services")
        {
        }
    }
}
