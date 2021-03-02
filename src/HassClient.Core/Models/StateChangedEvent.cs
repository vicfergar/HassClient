using HassClient.Helpers;
using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a state changed event.
    /// </summary>
    public class StateChangedEvent
    {
        /// <summary>
        /// Gets the entity id of the entity.
        /// </summary>
        [JsonProperty]
        public string EntityId { get; private set; }

        /// <summary>
        /// Gets or sets the entity domain of the entity.
        /// </summary>
        [JsonIgnore]
        public string Domain => EntityIdHelpers.GetDomain(this.EntityId);

        /// <summary>
        /// Gets the old state.
        /// </summary>
        [JsonProperty]
        public StateModel OldState { get; private set; }

        /// <summary>
        /// Gets the new state.
        /// </summary>
        [JsonProperty]
        public StateModel NewState { get; private set; }
    }
}
