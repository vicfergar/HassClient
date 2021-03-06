namespace HassClient.Models
{
    /// <summary>
    /// Represents a modifiable property from a model.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    public class ModifiableProperty<T> : IModifiableProperty
    {
        private T unmodifiedValue;

        private T currentValue;

        /// <summary>
        /// Gets or sets a value for the property.
        /// </summary>
        public T Value
        {
            get => this.currentValue;
            set
            {
                this.currentValue = value;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public bool HasPendingChange => !Equals(this.currentValue, this.unmodifiedValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        public ModifiableProperty(string name)
        {
            this.Name = name;
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            this.unmodifiedValue = this.currentValue;
        }

        /// <inheritdoc />
        public void DiscardPendingChange()
        {
            this.currentValue = this.unmodifiedValue;
        }
    }
}
