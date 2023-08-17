namespace HassClient.WS.Messages
{
    internal class PipelineRunSampleRateInput : IPipelineRunInput
    {
        public PipelineRunSampleRateInput(int sampleRate)
        {
            this.SampleRate = sampleRate;
        }

        public int SampleRate { get; set; }
    }
}
