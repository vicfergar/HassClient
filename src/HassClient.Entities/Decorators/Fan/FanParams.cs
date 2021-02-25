using Newtonsoft.Json;
using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents parameters used by service invocations from <see cref="FanEntity"/>.
    /// </summary>
    public class FanParams : FanOnParams
    {
        private float? speedPercentageStep;

        /// <summary>
        /// Gets or sets the speed as percentage for a fan that supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// <para>Should be between <c>0</c> and <c>100</c>.</para>
        /// </summary>
        [JsonProperty("percentage_step", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpeedPercentageStep
        {
            get => this.speedPercentageStep;
            set => this.speedPercentageStep = value.HasValue ? (float?)Math.Clamp(value.Value, 0, 100) : null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a fan that supports
        /// <see cref="FanFeatures.SupportOscillate"/> should oscillate.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Oscillating { get; set; }

        /// <summary>
        /// Gets or sets direction for a fan that supports <see cref="FanFeatures.SupportDirection"/>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FanDirections? Direction { get; set; }
    }
}
