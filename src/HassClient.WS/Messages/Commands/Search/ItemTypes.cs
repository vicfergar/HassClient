namespace HassClient
{
    /// <summary>
    /// Represents well-known Home Assistant item types used during relation search.
    /// </summary>
    public enum ItemTypes
    {
        /// <summary>
        /// Represents physical areas within a home.
        /// </summary>
        Area,

        /// <summary>
        /// Represents a Home Assistant automation.
        /// </summary>
        Automation,

        /// <summary>
        /// Represents a blueprint for creating automations in Home Assistant.
        /// </summary>
        AutomationBlueprint,

        /// <summary>
        /// Represents configuration data stored persistently by Home Assistant.
        /// </summary>
        ConfigEntry,

        /// <summary>
        /// Represents a device in Home Assistant.
        /// </summary>
        Device,

        /// <summary>
        /// Represents an entity in Home Assistant.
        /// </summary>
        Entity,

        /// <summary>
        /// Represents a floor within a home.
        /// </summary>
        Floor,

        /// <summary>
        /// Represents a group of items in Home Assistant.
        /// </summary>
        Group,

        /// <summary>
        /// Represents an integration in Home Assistant.
        /// </summary>
        Integration,

        /// <summary>
        /// Represents a label used in Home Assistant.
        /// </summary>
        Label,

        /// <summary>
        /// Represents a person in Home Assistant.
        /// </summary>
        Person,

        /// <summary>
        /// Represents a scene in Home Assistant.
        /// </summary>
        Scene,

        /// <summary>
        /// Represents a script in Home Assistant.
        /// </summary>
        Script,

        /// <summary>
        /// Represents a blueprint for creating scripts in Home Assistant.
        /// </summary>
        ScriptBlueprint,
    }
}
