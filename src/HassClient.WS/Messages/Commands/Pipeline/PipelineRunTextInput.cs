namespace HassClient.WS.Messages
{
    internal class PipelineRunTextInput : IPipelineRunInput
    {
        public PipelineRunTextInput(string text)
        {
            this.Text = text;
        }

        public string Text { get; set; }
    }
}
