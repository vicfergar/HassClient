namespace HassClient.Net.WSMessages
{
    internal class GetStatesMessage : BaseOutgoingMessage
    {
        public GetStatesMessage()
            : base("get_states")
        {
        }
    }
}
