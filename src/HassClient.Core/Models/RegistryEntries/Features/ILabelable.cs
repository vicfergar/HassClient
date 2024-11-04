using System.Collections.Generic;
using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines properties for a registry entry that can be labeled.
    /// </summary>
    public interface ILabelable
    {
        /// <summary>
        /// Gets the labels associated with this entry.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        ICollection<string> Labels { get; }
    }
}
