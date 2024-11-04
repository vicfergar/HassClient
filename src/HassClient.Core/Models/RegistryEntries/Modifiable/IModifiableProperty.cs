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
        bool HasPendingChanges { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this property should always be included
        /// in server updates, even when unchanged.
        /// </summary>
        bool AlwaysIncludeInUpdate { get; }

        /// <summary>
        /// Save pending change and clears the <see cref="HasPendingChanges"/> flag.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Discards any pending change and clears the <see cref="HasPendingChanges"/> flag.
        /// </summary>
        void DiscardPendingChanges();
    }
}
