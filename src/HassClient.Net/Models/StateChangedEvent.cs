using HassClient.Net.Helpers;
using Newtonsoft.Json;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents a state changed event.
    /// </summary>
    public class StateChangedEvent
    {
        /// <summary>
        /// Gets or sets the entity id of the entity.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity domain of the entity.
        /// </summary>
        [JsonIgnore]
        public string Domain => HassHelpers.GetDomain(this.EntityId);

        /// <summary>
        /// Gets or sets the old state.
        /// </summary>
        public StateModel OldState { get; set; }

        /// <summary>
        /// Gets or sets the new state.
        /// </summary>
        public StateModel NewState { get; set; }
    }
}
