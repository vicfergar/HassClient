using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents fan supported features flags.
    /// </summary>
    [Flags]
    public enum FanFeatures
    {
        /// <summary>
        /// No fan feature is supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Fan entity supports speed setting.
        /// </summary>
        SupportSetSpeed = 1,

        /// <summary>
        /// Fan entity supports oscillation feature.
        /// </summary>
        SupportOscillate = 2,

        /// <summary>
        /// Fan entity supports direction feature.
        /// </summary>
        SupportDirection = 4,

        /// <summary>
        /// Fan entity supports preset modes.
        /// </summary>
        SupportPresetMode = 8,
    }
}
