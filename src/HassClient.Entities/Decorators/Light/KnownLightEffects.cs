namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents known lights effects. Useful to reduce use of strings.
    /// </summary>
    public enum KnownLightEffects
    {
        /// <summary>
        /// Used to represent a light effect not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// No light effect is active or the light does not support <see cref="LightFeatures.SupportEffects"/>.
        /// </summary>
        None,

        /// <summary>
        /// The light color loop effect.
        /// </summary>
        ColorLoop,

        /// <summary>
        /// The light rainbow effect.
        /// </summary>
        Rainbow,

        /// <summary>
        /// The light random effect.
        /// </summary>
        Random,
    }
}
