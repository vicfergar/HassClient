namespace HassClient.Models
{
    /// <summary>
    /// Defines the disabling source of an entity in Home Assistant.
    /// <para>
    /// More information at <see href="https://developers.home-assistant.io/docs/entity_registry_disabled_by/"/>.
    /// </para>
    /// </summary>
    public enum DisabledByEnum
    {
        /// <summary>
        /// The entity is not disabled.
        /// </summary>
        None,

        /// <summary>
        /// The entity has been disabled by the configuration entry.
        /// </summary>
        ConfigEntry,

        /// <summary>
        /// The entity has been disabled by the entity device.
        /// </summary>
        Device,

        /// <summary>
        /// The entity has been disabled by the Home Assistant.
        /// </summary>
        Hass,

        /// <summary>
        /// The entity has been disabled by an user.
        /// </summary>
        User,

        /// <summary>
        /// The entity has been disabled by the entity integration.
        /// </summary>
        Integration,
    }
}
