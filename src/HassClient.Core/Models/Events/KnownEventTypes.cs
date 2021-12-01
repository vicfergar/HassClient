using System.Runtime.Serialization;

namespace HassClient.Models
{
    /// <summary>
    /// Collection of built-in event types available in any Home Assistant instance by default.
    /// </summary>
    public enum KnownEventTypes
    {
        /// <summary>
        /// Default filter used in event subscription. When used, every kind of event will be received.
        /// </summary>
        [EnumMember(Value = Event.AnyEventFilter)]
        Any = 0,

        /// <summary>
        /// Fired when area registry have been updated and might have changed.
        /// </summary>
        AreaRegistryUpdated,

        /// <summary>
        /// Fired when automations have been reloaded and thus might have changed.
        /// </summary>
        AutomationReloaded,

        /// <summary>
        /// Fired every time a service is called.
        /// </summary>
        CallService,

        /// <summary>
        /// Fired when a new integration has been loaded and initialized.
        /// </summary>
        ComponentLoaded,

        /// <summary>
        /// Fired when the core configuration have been updated and might have changed.
        /// </summary>
        CoreConfigUpdated,

        /// <summary>
        /// Fired when the device registry have been updated and might have changed.
        /// </summary>
        DeviceRegistryUpdated,

        /// <summary>
        /// Fired when the entity registry have been updated and might have changed.
        /// </summary>
        EntityRegistryUpdated,

        /// <summary>
        /// Fired when home assistant instance have been closed.
        /// </summary>
        [EnumMember(Value = "homeassistant_close")]
        HomeAssistantClose,

        /// <summary>
        /// Fired when home assistant instance have performed the latest writing operation before closing.
        /// </summary>
        [EnumMember(Value = "homeassistant_final_write")]
        HomeAssistantFinalWrite,

        /// <summary>
        /// Fired when home assistant instance have been started.
        /// </summary>
        [EnumMember(Value = "homeassistant_start")]
        HomeAssistantStart,

        /// <summary>
        /// Fired when home assistant instance have been stopped.
        /// </summary>
        [EnumMember(Value = "homeassistant_stop")]
        HomeAssistantStop,

        /// <summary>
        /// Fired when Logbook entry is registered.
        /// </summary>
        LogbookEntry,

        /// <summary>
        /// Fired when Lovelace UI have been reloaded and thus might have changed.
        /// </summary>
        LovelaceUpdated,

        /// <summary>
        /// Fired when panels have been reloaded and thus might have changed.
        /// </summary>
        PanelsUpdated,

        /// <summary>
        /// Fired when persistent notifications have been reloaded and thus might have changed.
        /// </summary>
        PersistentNotificationsUpdated,

        /// <summary>
        /// Fired when a new platform has been discovered by the discovery component.
        /// </summary>
        PlatformDiscovered,

        /// <summary>
        /// Fired by the service handler to indicate the service is done.
        /// </summary>
        ServiceExecuted,

        /// <summary>
        /// Fired when a new service has been registered within Home Assistant
        /// </summary>
        ServiceRegistered,

        /// <summary>
        /// Fired when a new service has been removed from Home Assistant
        /// </summary>
        ServiceRemoved,

        /// <summary>
        /// Fired when scenes have been reloaded and thus might have changed.
        /// </summary>
        SceneReloaded,

        /// <summary>
        /// Fired when a state changes.
        /// </summary>
        StateChanged,

        /// <summary>
        /// Fired when themes have been updated and thus might have changed.
        /// </summary>
        ThemesUpdated,

        /// <summary>
        /// Fired every second by the timer and contains the current time.
        /// </summary>
        TimeChanged,

        /// <summary>
        /// Fired when Home Assistant timer gets out of sync.
        /// </summary>
        TimerOutOfSync,

        /// <summary>
        /// Fired when a Home Assistant user is added.
        /// </summary>
        UserAdded,

        /// <summary>
        /// Fired when a Home Assistant user is removed.
        /// </summary>
        UserRemoved,
    }
}
