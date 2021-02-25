using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents light supported features.
    /// </summary>
    [Flags]
    public enum LightFeatures
    {
        /// <summary>
        /// No light feature is supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Light entity supports brightness control.
        /// </summary>
        [Obsolete("Replaced by color modes. It will be removed in future versions of Home Assistant")]
        SupportBrightness = 1,

        /// <summary>
        /// Light entity supports color temperature control.
        /// </summary>
        [Obsolete("Replaced by color modes. It will be removed in future versions of Home Assistant")]
        SupportColorTemp = 2,

        /// <summary>
        /// Light entity supports light control.
        /// </summary>
        SupportEffects = 4,

        /// <summary>
        /// Light entity supports flash control.
        /// </summary>
        SupportFlash = 8,

        /// <summary>
        /// Light entity supports color control.
        /// </summary>
        [Obsolete("Replaced by color modes. It will be removed in future versions of Home Assistant")]
        SupportColor = 16,

        /// <summary>
        /// Light entity supports transitions.
        /// </summary>
        SupportTransition = 32,

        /// <summary>
        /// Light entity supports color white value control.
        /// </summary>
        [Obsolete("Replaced by color modes. It will be removed in future versions of Home Assistant")]
        SupportWhiteValue = 128,
    }
}
