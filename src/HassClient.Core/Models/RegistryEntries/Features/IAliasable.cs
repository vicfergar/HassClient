using System.Collections.Generic;
using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines properties for a registry entry that can be aliased.
    /// </summary>
    public interface IAliasable
    {
        /// <summary>
        /// Gets the aliases associated with this entry.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        ICollection<string> Aliases { get; }
    }
}
