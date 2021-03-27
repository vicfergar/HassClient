using System.Diagnostics.CodeAnalysis;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a list of known domains. Useful to reduce use of strings.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1602:Enumeration items should be documented",
        Justification = "Due to the nature of the list, it is not necessary to document each field.")]
    public enum KnownDomains
    {
        /// <summary>
        /// Used to represent a domain not defined within this enum.
        /// </summary>
        Undefined = 0,

        Adguard,
        AirQuality,
        AlarmControlPanel,
        Automation,
        BinarySensor,
        Calendar,
        Camera,
        Cast,
        Climate,
        Cloud,
        Counter,
        Cover,
        DeviceTracker,
        Esphome,
        Fan,
        Filesize,
        Frontend,
        Generic,
        GenericThermostat,
        Group,
        Hassio,
        Homeassistant,
        Html5,
        Humidifier,
        ImageProcessing,
        InputBoolean,
        InputDatetime,
        InputNumber,
        InputSelect,
        InputText,
        Light,
        Lock,
        Logbook,
        Logger,
        Mailbox,
        MediaPlayer,
        MQTT,
        Notify,
        Number,
        PersistentNotification,
        Person,
        PythonScript,
        Recorder,
        Remote,
        Scene,
        Script,
        Sensor,
        Speedtestdotnet,
        Stream,
        Sun,
        Switch,
        SystemLog,
        Template,
        Timer,
        TTS,
        Vacuum,
        WakeOnLan,
        WaterHeater,
        Weather,
        WebosTV,
        XiaomiMiio,
        ZHA,
        Zone,
    }
}
