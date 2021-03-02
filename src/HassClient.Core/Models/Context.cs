using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an entity state's context.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Gets the ID of this context.
        /// </summary>
        [JsonProperty]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the Parent Context ID if this element is a child of another context, otherwise <see langword="null" />.
        /// </summary>
        [JsonProperty]
        public string ParentId { get; private set; }

        /// <summary>
        /// Gets the User ID of this element, or <see langword="null" /> for the default user or no user.
        /// </summary>
        [JsonProperty]
        public string UserId { get; private set; }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Context)}: {this.Id}{(!string.IsNullOrWhiteSpace(this.ParentId) ? " / Parent: " + this.ParentId : string.Empty)}";
    }
}
