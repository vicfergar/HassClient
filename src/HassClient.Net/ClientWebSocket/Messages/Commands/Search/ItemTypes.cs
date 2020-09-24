namespace HassClient.Net
{
    /// <summary>
    /// Well known Home Assistant item types used during relation search.
    /// </summary>
    public enum ItemTypes
    {
        /// <summary>
        /// Physical areas of a home.
        /// </summary>
        Area,

        /// <summary>
        /// Home Assistant automations.
        /// </summary>
        Automation,

        /// <summary>
        /// Configuration data that are persistently stored by Home Assistant.
        /// </summary>
        ConfigEntry,

        /// <summary>
        /// Home Assistant device.
        /// </summary>
        Device,

        /// <summary>
        /// Home Assistant entity.
        /// </summary>
        Entity,

        /// <summary>
        /// Home Assistant group.
        /// </summary>
        Group,

        /// <summary>
        /// Home Assistant scene.
        /// </summary>
        Scene,

        /// <summary>
        /// Home Assistant script.
        /// </summary>
        Script,
    }
}
