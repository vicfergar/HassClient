using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a list of known states. Useful to reduce use of strings.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1602:Enumeration items should be documented",
        Justification = "Due to the nature of the list, it is not necessary to document each field.")]
    public enum KnownStates
    {
        /// <summary>
        /// Used to represent a state not defined within this enum.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The entity state is not available.
        /// </summary>
        Unavailable,

        /// <summary>
        /// The entity state is unknown.
        /// </summary>
        Unknown,

        AboveHorizon,
        Active,
        Armed,
        ArmedAway,
        ArmedCustomBypass,
        ArmedHome,
        ArmedNight,
        Arming,
        Auto,
        BackedUp,
        BellowHorizon,
        Cleaning,
        [EnumMember(Value = "clear-night")]
        ClearNight,
        Closed,
        Closing,
        Cloudy,
        Configure,
        Configured,
        Cool,
        Dead,
        Disarmed,
        Disarming,
        Discharging,
        Docked,
        Dry,
        Eco,
        Error,
        Exceptional,
        FanOnly,
        Far,
        Fog,
        Hail,
        Hans,
        Heat,
        HeatCool,
        Home,
        Idle,
        Initializing,
        Lightning,
        [EnumMember(Value = "lightning-rainy")]
        LightningRainy,
        Locked,
        None,
        NotHome,
        Notifying,
        Off,
        Ok,
        On,
        Open,
        Opening,
        Partlycloudy,
        Paused,
        Pending,
        Playing,
        Pouring,
        PriorityOnly,
        Problem,
        Rainy,
        Ready,
        Recording,
        Returning,
        Sleeping,
        Snowy,
        [EnumMember(Value = "snowy-rainy")]
        SnowyRainy,
        Standby,
        Still,
        Stopped,
        Streaming,
        Sunny,
        Triggered,
        Unlocked,
        Vibrate,
        Windy,
        [EnumMember(Value = "windy-variant")]
        WindyVariant,
        Zoning,
    }
}
