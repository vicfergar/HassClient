namespace HassClient.WS.Messages
{
    internal class GetStatesMessage : BaseOutgoingMessage
    {
        public GetStatesMessage()
            : base("get_states")
        {
        }
    }
}
