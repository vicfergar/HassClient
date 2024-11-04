using System;
using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines properties for tracking creation and modification timestamps.
    /// </summary>
    public interface ITimeTracked
    {
        /// <summary>
        /// Gets the timestamp when this entry was created.
        /// </summary>
        [JsonProperty]
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Gets the timestamp when this entry was last modified.
        /// </summary>
        [JsonProperty]
        DateTimeOffset ModifiedAt { get; }
    }
}
