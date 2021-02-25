namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents known fan speeds. Useful to reduce use of strings.
    /// </summary>
    public enum KnownFanSpeeds
    {
        /// <summary>
        /// Used to represent a fan speed not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// The fan does not support <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        None,

        /// <summary>
        /// The fan is in the off state.
        /// </summary>
        Off,

        /// <summary>
        /// The fan minimum speed.
        /// </summary>
        Low,

        /// <summary>
        /// The fan medium speed.
        /// </summary>
        Medium,

        /// <summary>
        /// The fan maximum speed.
        /// </summary>
        High,
    }
}
