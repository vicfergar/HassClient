using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines information related with a Home Assistant 'Assist Pipeline' definition.
    /// </summary>
    public class PipelineInfo
    {
        /// <summary>
        /// Gets the pipeline conversation engine.
        /// </summary>
        [JsonProperty("conversation_engine")]
        public string ConversationEngine { get; private set; }

        /// <summary>
        /// Gets the pipeline conversation defined language.
        /// </summary>
        [JsonProperty("conversation_language")]
        public string ConversationLanguage { get; private set; }

        /// <summary>
        /// Gets the pipeline defined language.
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; private set; }

        /// <summary>
        /// Gets the pipeline name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the SpeechToText engine.
        /// </summary>
        [JsonProperty("stt_engine")]
        public string STTEngine { get; private set; }

        /// <summary>
        /// Gets the SpeechToText defined language.
        /// </summary>
        [JsonProperty("stt_language")]
        public string STTLanguage { get; private set; }

        /// <summary>
        /// Gets the TextToSpeach engine.
        /// </summary>
        [JsonProperty("tts_engine")]
        public string TTSEngine { get; private set; }

        /// <summary>
        /// Gets the TextToSpeach defined language.
        /// </summary>
        [JsonProperty("tts_language")]
        public string TTSLanguage { get; private set; }

        /// <summary>
        /// Gets the name of TextToSpeach defined voice.
        /// </summary>
        [JsonProperty("tts_voice")]
        public string TTSVoice { get; private set; }

        /// <summary>
        /// Gets the name of pipeline id.
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; private set; }
    }
}
