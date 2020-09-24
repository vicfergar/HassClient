namespace HassClient.Net.WSMessages
{
    internal class ListManifestsMessage : BaseOutgoingMessage
    {
        public ListManifestsMessage()
            : base("manifest/list")
        {
        }
    }
}
