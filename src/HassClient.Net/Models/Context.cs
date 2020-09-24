namespace HassClient.Net.Models
{
    /// <summary>
    /// Represents an entity state's context.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Gets or sets the ID of this context.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Parent Context ID if this element is a child of another context, otherwise <see langword="null" />.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the User ID of this element, or <see langword="null" /> for the default user or no user.
        /// </summary>
        public string UserId { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Context)}: {this.Id}{(!string.IsNullOrWhiteSpace(this.ParentId) ? " / Parent: " + this.ParentId : string.Empty)}";
    }
}
