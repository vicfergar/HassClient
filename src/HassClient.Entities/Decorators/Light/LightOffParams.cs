using Newtonsoft.Json;
using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents parameters used by service invocations from <see cref="LightEntity"/>.
    /// </summary>
    public class LightOffParams
    {
        private uint? transition;

        /// <summary>
        /// Gets or sets the duration in seconds it takes to get to next state.
        /// <para>Should be between <c>0</c> and <c>300</c>.</para>
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? Transition
        {
            get => this.transition;
            set => this.transition = value.HasValue ? (uint?)Math.Min(value.Value, 300) : null;
        }

        /// <summary>
        /// Gets or sets a value indicating if the light should flash.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FlashMode? Flash { get; set; }
    }
}
