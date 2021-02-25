namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Light profiles as defined in
    /// <see href="https://github.com/home-assistant/core/blob/dev/homeassistant/components/light/light_profiles.csv"/>.
    /// </summary>
    public enum KnownLightProfiles
    {
        /// <summary>
        /// Used to represent a light profile not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// The relax profile.
        /// </summary>
        Relax,

        /// <summary>
        /// The concentrate profile.
        /// </summary>
        Concentrate,

        /// <summary>
        /// The energize profile.
        /// </summary>
        Energize,

        /// <summary>
        /// The reading profile.
        /// </summary>
        Reading,
    }
}
