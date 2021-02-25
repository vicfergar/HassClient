using Newtonsoft.Json;
using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents parameters used by service invocations from <see cref="FanEntity"/>.
    /// </summary>
    public class FanOnParams
    {
        private float? speedPercentage;

        /// <summary>
        /// Gets or sets the speed as percentage for the fan.
        /// <para>Should be between <c>0</c> and <c>100</c>.</para>
        /// </summary>
        [JsonProperty("percentage", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpeedPercentage
        {
            get => this.speedPercentage;
            set => this.speedPercentage = value.HasValue ? (float?)Math.Clamp(value.Value, 0, 100) : null;
        }

        /// <summary>
        /// Gets or sets the name of a fan speed to use.
        /// <para>
        /// It is recommended to use <see cref="Speed"/> property instead when possible to reduce
        /// use of strings.
        /// </para>
        /// </summary>
        [JsonProperty("speed", NullValueHandling = NullValueHandling.Ignore)]
        public string SpeedName { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="KnownFanSpeeds"/>.
        /// </summary>
        [JsonIgnore]
        public KnownFanSpeeds? Speed
        {
            get => FanEntity.knownFanSpeedCache.AsEnum(this.SpeedName);
            set => this.SpeedName = value.HasValue && value != KnownFanSpeeds.Unknown
                ? FanEntity.knownFanSpeedCache.AsString(value.Value)
                : null;
        }

        /// <summary>
        /// Gets or sets the name of a fan preset mode to use.
        /// <para>
        /// It is recommended to use <see cref="PresetMode"/> property instead when possible to reduce
        /// use of strings.
        /// </para>
        /// </summary>
        [JsonProperty("preset_mode", NullValueHandling = NullValueHandling.Ignore)]
        public string PresetModeName { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="KnownFanPresetModes"/>.
        /// </summary>
        [JsonIgnore]
        public KnownFanPresetModes? PresetMode
        {
            get => FanEntity.knownFanPresetsCache.AsEnum(this.PresetModeName);
            set => this.PresetModeName = value.HasValue && value != KnownFanPresetModes.Unknown
                ? FanEntity.knownFanPresetsCache.AsString(value.Value)
                : null;
        }
    }
}
