namespace HassClient.WS.Messages
{
    internal class GetManifestMessage : BaseOutgoingMessage
    {
        /// <summary>
        /// Gets or sets the name of the integration to query.
        /// </summary>
        public string Integration { get; set; }

        public GetManifestMessage()
            : base("manifest/get")
        {
        }
    }
}
