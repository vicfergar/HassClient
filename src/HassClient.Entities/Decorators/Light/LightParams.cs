using HassClient.Models;
using HassClient.Helpers;
using Newtonsoft.Json;
using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents parameters used by service invocations from <see cref="LightEntity"/>.
    /// </summary>
    public class LightParams : LightOffParams
    {
        private static KnownEnumCache<KnownLightProfiles> knownLightProfilesCache = new KnownEnumCache<KnownLightProfiles>();

        private int? brightness;

        private int? brightnessStep;

        private int? brightnessStepValue;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private RGBColor rgb_color => this.Color as RGBColor;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private NameColor color_name => this.Color as NameColor;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private HSColor hs_color => this.Color as HSColor;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private XYColor xy_color => this.Color as XYColor;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private uint? color_temp => (this.Color as MiredsTemperatureColor)?.Mireds;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private uint? kelvin => (this.Color as KelvinTemperatureColor)?.Kelvins;

        /// <summary>
        /// Gets or sets the color for lights that supports <see cref="LightFeatures.SupportColor"/>
        /// or <see cref="LightFeatures.SupportColorTemp"/>.
        /// </summary>
        [JsonIgnore]
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the level of white for lights that supports <see cref="LightFeatures.SupportWhiteValue"/>.
        /// <para>Should be between <c>0</c> and <c>255</c>.</para>
        /// </summary>
        [JsonProperty("white_value", NullValueHandling = NullValueHandling.Ignore)]
        public byte? WhiteLevel { get; set; }

        /// <summary>
        /// Gets or sets the brightness for lights that supports <see cref="LightFeatures.SupportBrightness"/>.
        /// <para>
        /// Should be between <c>0</c> and <c>255</c>, where <c>0</c> turns the light off,
        /// <c>1</c> is the minimum brightness and <c>255</c> is the maximum brightness supported by the light.
        /// </para>
        /// </summary>
        [JsonProperty("brightness", NullValueHandling = NullValueHandling.Ignore)]
        public byte? BrightnessValue { get; set; }

        /// <summary>
        /// Gets or sets the percentage of full brightness for lights that supports <see cref="LightFeatures.SupportBrightness"/>.
        /// <para>
        /// Should be between <c>0</c> and <c>100</c>, where <c>0</c> turns the light off,
        /// <c>1</c> is the minimum brightness and <c>100</c> is the maximum brightness supported by the light.
        /// </para>
        /// </summary>
        [JsonProperty("brightness_pct", NullValueHandling = NullValueHandling.Ignore)]
        public int? Brightness
        {
            get => this.brightness;
            set => this.brightness = value.HasValue ? (int?)Math.Clamp(value.Value, 0, 100) : null;
        }

        /// <summary>
        /// Gets or sets a value indicating a change brightness by a percentage relative to the current light
        /// <see cref="LightEntity.Brightness"/> for lights that supports <see cref="LightFeatures.SupportBrightness"/>.
        /// <para>
        /// Should be between <c>-100</c> and <c>100</c>.
        /// </para>
        /// </summary>
        [JsonProperty("brightness_step_pct", NullValueHandling = NullValueHandling.Ignore)]
        public int? BrightnessStep
        {
            get => this.brightnessStep;
            set => this.brightnessStep = value.HasValue ? (int?)Math.Clamp(value.Value, -100, 100) : null;
        }

        /// <summary>
        /// Gets or sets a value indicating a change brightness by an amount relative to the current light
        /// <see cref="LightEntity.BrightnessValue"/> for lights that supports <see cref="LightFeatures.SupportBrightness"/>.
        /// <para>
        /// Should be between <c>-255</c> and <c>255</c>.
        /// </para>
        /// </summary>
        [JsonProperty("brightness_step", NullValueHandling = NullValueHandling.Ignore)]
        public int? BrightnessStepValue
        {
            get => this.brightnessStepValue;
            set => this.brightnessStepValue = value.HasValue ? (int?)Math.Clamp(value.Value, -255, 255) : null;
        }

        /// <summary>
        /// Gets or sets the name of a light profile to use.
        /// <para>
        /// It is recommended to use <see cref="Profile"/> property instead when possible to reduce
        /// use of strings.
        /// </para>
        /// </summary>
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="KnownLightProfiles"/> to use.
        /// </summary>
        [JsonIgnore]
        public KnownLightProfiles? Profile
        {
            get => knownLightProfilesCache.AsEnum(this.ProfileName);
            set => this.ProfileName = value.HasValue && value != KnownLightProfiles.Unknown
                ? knownLightProfilesCache.AsString(value.Value)
                : null;
        }

        /// <summary>
        /// Gets or sets the name of a light effect to use.
        /// <para>
        /// It is recommended to use <see cref="Effect"/> property instead when possible to reduce
        /// use of strings.
        /// </para>
        /// </summary>
        [JsonProperty("effect", NullValueHandling = NullValueHandling.Ignore)]
        public string EffectName { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="KnownLightEffects"/>.
        /// </summary>
        [JsonIgnore]
        public KnownLightEffects? Effect
        {
            get => LightEntity.knownLightEffectCache.AsEnum(this.EffectName);
            set => this.EffectName = value.HasValue && value != KnownLightEffects.Unknown
                ? LightEntity.knownLightEffectCache.AsString(value.Value)
                : null;
        }
    }
}
