using System.Diagnostics.CodeAnalysis;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a list of known services. Useful to reduce use of strings.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1602:Enumeration items should be documented",
        Justification = "Due to the nature of the list, it is not necessary to document each field.")]
    public enum KnownServices
    {
        /// <summary>
        /// Used to represent a service not defined within this enum.
        /// </summary>
        Undefined = 0,

        // Common (Shared between several domains)
        Create,
        Decrement,
        Dismiss,
        Increment,
        Pause,
        Record,
        Reload,
        Remove,
        SendCommand,
        SetValue,
        Start,
        Stop,
        Toggle,
        TurnOff,
        TurnOn,

        // Adguard
        AddUrl,
        DisableUrl,
        EnableUrl,
        Refresh,
        RemoveUrl,

        // Automation
        Trigger,

        // Camera
        DisableMotionDetection,
        EnableMotionDetection,
        PlayStream,
        Snapshot,

        // Cast
        ShowLovelaceView,

        // Climate
        SetAuxHeat,
        SetFanMode,
        SetHumidity,
        SetHvacMode,
        SetPresetMode,
        SetSwingMode,
        SetTemperature,

        // Cloud
        RemoteConnect,
        RemoteDisconnect,

        // Counter
        Configure,
        Reset,

        // Cover
        CloseCover,
        CloseCoverTilt,
        OpenCover,
        OpenCoverTilt,
        SetCoverPosition,
        SetCoverTiltPosition,
        StopCover,
        StopCoverTilt,
        ToggleCoverTilt,

        // DeviceTracker
        See,

        // Fan
        Oscillate,
        SetDirection,
        SetSpeed,

        // Frontend
        ReloadThemes,
        SetTheme,

        // Group
        Set,

        // Hassio
        AddonRestart,
        AddonStart,
        AddonStdin,
        AddonStop,
        HostReboot,
        HostShutdown,
        RestoreFull,
        RestorePartial,
        SnapshotFull,
        SnapshotPartial,

        // Homeassistant
        CheckConfig,
        ReloadCoreConfig,
        Restart,
        SetLocation,
        UpdateEntity,

        // InputDatetime
        SetDatetime,

        // InputSelect
        SelectFirst,
        SelectLast,
        SelectNext,
        SelectOption,
        SelectPrevious,
        SetOptions,

        // Lock
        Lock,
        Open,
        Unlock,

        // Logbook
        Log,

        // Logger
        SetDefaultLevel,
        SetLevel,

        // MediaPlayer
        ClearPlaylist,
        MediaNextTrack,
        MediaPause,
        MediaPlay,
        MediaPlayPause,
        MediaPreviousTrack,
        MediaSeek,
        MediaStop,
        PlayMedia,
        RepeatSet,
        SelectSoundMode,
        SelectSource,
        ShuffleSet,
        VolumeDown,
        VolumeMute,
        VolumeSet,
        VolumeUp,

        // MQTT
        Dump,
        Publish,

        // Notify
        Notify,
        PersistentNotification,

        // PersistentNotification
        MarkRead,

        // Recorder
        Purge,

        // Remote
        DeleteCommand,
        LearnCommand,

        // Scene
        Apply,

        // Speedtestdotnet
        Speedtest,

        // SystemLog
        Clear,
        Write,

        // Timer
        Cancel,
        Finish,

        // TTS
        ClearCache,
        CloudSay,
        GoogleSay,

        // Vacuum
        CleanSpot,
        Locate,
        ReturnToBase,
        SetFanSpeed,
        StartPause,

        // WakeOnLan
        SendMagicPacket,

        // WebosTV
        Button,
        Command,
        SelectSoundOutput,

        // XiaomiMiio
        VacuumCleanSegment,
        VacuumCleanZone,
        VacuumGoto,
        VacuumRemoteControlMove,
        VacuumRemoteControlMoveStep,
        VacuumRemoteControlStart,
        VacuumRemoteControlStop,

        // ZHA
        IssueZigbeeClusterCommand,
        IssueZigbeeGroupCommand,
        Permit,
        SetZigbeeClusterAttribute,
        WarningDeviceSquawk,
        WarningDeviceWarn,
    }
}
