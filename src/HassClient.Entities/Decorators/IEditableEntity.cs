namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity with editable properties.
    /// </summary>
    public interface IEditableEntity
    {
        /// <summary>
        /// Gets a value indicating whether the entity is editable.
        /// </summary>
        bool IsEditable { get; }
    }
}
