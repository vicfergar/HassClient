namespace HassClient.WS.Messages
{
    internal class ListManifestsMessage : BaseOutgoingMessage
    {
        public ListManifestsMessage()
            : base("manifest/list")
        {
        }
    }
}
