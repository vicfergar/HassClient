using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Defines properties for an entity registry entry.
    /// </summary>
    public interface IEntityEntry
    {
        /// <summary>
        /// Gets the identifier of the entity.
        /// </summary>
        [JsonIgnore]
        string EntityId { get; }
    }
}
