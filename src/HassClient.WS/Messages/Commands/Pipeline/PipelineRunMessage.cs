using Newtonsoft.Json;

namespace HassClient.WS.Messages
{
    internal class PipelineRunMessage : BaseOutgoingMessage
    {
        public PipelineRunMessage()
            : base("assist_pipeline/run")
        {
        }

        /// <summary>
        /// The first stage to run.
        /// </summary>
        public StageTypes StartStage { get; set; }

        /// <summary>
        /// The last stage to run.
        /// </summary>
        public StageTypes EndStage { get; set; }

        /// <summary>
        /// Depends on <see cref="StartStage"/>.
        /// <para>
        /// For <see cref="StageTypes.STT"/>, it should be an <see cref="PipelineRunSampleRateInput"/>.
        /// </para>
        /// <para>
        /// For <see cref="StageTypes.Intent"/> and <see cref="StageTypes.TTS"/>, it should be an <see cref="PipelineRunTextInput"/>.
        /// </para>
        /// </summary>
        public IPipelineRunInput Input { get; set; }

        /// <summary>
        /// ID of the pipeline.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Pipeline { get; set; }

        /// <summary>
        /// Unique id for conversation. <see href="https://developers.home-assistant.io/docs/intent_conversation_api#conversation-id"/>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConversationId { get; set; }

        /// <summary>
        /// Number of seconds before pipeline times out (default: 30).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? Timeout { get; set; }
    }
}
