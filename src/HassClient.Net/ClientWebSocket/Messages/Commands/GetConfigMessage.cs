namespace HassClient.Net.WSMessages
{
    internal class GetConfigMessage : BaseOutgoingMessage
    {
        public GetConfigMessage()
            : base("get_config")
        {
        }
    }
}
