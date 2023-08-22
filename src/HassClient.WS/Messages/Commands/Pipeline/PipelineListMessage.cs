namespace HassClient.WS.Messages
{
    internal class PipelineListMessage : BaseOutgoingMessage
    {
        public PipelineListMessage()
            : base("assist_pipeline/pipeline/list")
        {
        }
    }
}
