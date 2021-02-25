namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents known fan preset modes. Useful to reduce use of strings.
    /// </summary>
    public enum KnownFanPresetModes
    {
        /// <summary>
        /// Used to represent a fan preset mode not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// No preset mode is active or the fan does not support <see cref="FanFeatures.SupportPresetMode"/>.
        /// </summary>
        None,

        /// <summary>
        /// The automatic preset mode.
        /// </summary>
        Auto,

        /// <summary>
        /// The smart preset mode.
        /// </summary>
        Smart,

        /// <summary>
        /// The whoosh preset mode.
        /// </summary>
        Whoosh,

        /// <summary>
        /// The ECO preset mode.
        /// </summary>
        Eco,

        /// <summary>
        /// The breeze preset mode.
        /// </summary>
        Breeze,

        /// <summary>
        /// The sleep preset mode.
        /// </summary>
        Sleep,

        /// <summary>
        /// The on preset mode.
        /// </summary>
        On,
    }
}
