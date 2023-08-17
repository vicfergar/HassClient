namespace HassClient
{
    /// <summary>
    /// Well known Home Assistant stage types used during pipeline run.
    /// </summary>
    public enum StageTypes
    {
        /// <summary>
        /// Speech to Text.
        /// </summary>
        STT,

        /// <summary>
        /// Intent <see href="https://developers.home-assistant.io/docs/intent_index"/>.
        /// </summary>
        Intent,

        /// <summary>
        /// Text to Speech.
        /// </summary>
        TTS,
    }
}
