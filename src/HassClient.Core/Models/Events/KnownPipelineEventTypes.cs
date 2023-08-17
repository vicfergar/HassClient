using System.Runtime.Serialization;

namespace HassClient.Models
{
    /// <summary>
    /// Collection of built-in pipeline event types. See <see href="https://developers.home-assistant.io/docs/voice/pipelines/#events"/>.
    /// </summary>
    public enum KnownPipelineEventTypes
    {
        /// <summary>
        /// Used to represent a type not defined within this enum.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Start of pipeline run.
        /// </summary>
        [EnumMember(Value = "run-start")]
        RunStart,

        /// <summary>
        /// End of pipeline run.
        /// </summary>
        [EnumMember(Value = "run-end")]
        RunEnd,

        /// <summary>
        /// Start of speech to text.
        /// </summary>
        [EnumMember(Value = "stt-start")]
        STTStart,

        /// <summary>
        /// End of speech to text.
        /// </summary>
        [EnumMember(Value = "stt-end")]
        STTEnd,

        /// <summary>
        /// Start of intent recognition.
        /// </summary>
        [EnumMember(Value = "intent-start")]
        IntentStart,

        /// <summary>
        /// End of intent recognition.
        /// </summary>
        [EnumMember(Value = "intent-end")]
        IntentEnd,

        /// <summary>
        /// Start of text to speech.
        /// </summary>
        [EnumMember(Value = "tts-start")]
        TTSStart,

        /// <summary>
        /// End of text to speech.
        /// </summary>
        [EnumMember(Value = "tts-end")]
        TTSEnd,

        /// <summary>
        /// Error in pipeline.
        /// </summary>
        Error,
    }
}
