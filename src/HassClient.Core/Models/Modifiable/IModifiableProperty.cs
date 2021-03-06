namespace HassClient.Models
{
    /// <summary>
    /// Represents a modifiable property from a model.
    /// </summary>
    public interface IModifiableProperty
    {
        /// <summary>
        /// Gets the property name. Used to update only modified properties.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the property value has been changed.
        /// </summary>
        bool HasPendingChange { get; }

        /// <summary>
        /// Save pending change and clears the <see cref="HasPendingChange"/> flag.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Discards any pending change and clears the <see cref="HasPendingChange"/> flag.
        /// </summary>
        void DiscardPendingChange();
    }
}
