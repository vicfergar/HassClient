using System;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a modifiable property from a model.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1648:inheritdoc should be used with inheriting class", Justification = "Inherits document from base constructor")]
    public class ModifiableProperty<T> : ModifiablePropertyBase<T>
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
                this.ValidateValue(value);
                this.currentValue = value;
            }
        }

        /// <inheritdoc />
        public override bool HasPendingChanges => !Equals(this.currentValue, this.unmodifiedValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableProperty{T}"/> class.
        /// </summary>
        /// <inheritdoc/>
        public ModifiableProperty(string name, bool alwaysIncludeInUpdate = false)
            : base(name, alwaysIncludeInUpdate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableProperty{T}"/> class.
        /// </summary>
        /// <inheritdoc/>
        public ModifiableProperty(string name, Func<T, bool> validationFunc, string validationExceptionMessage = null, bool alwaysIncludeInUpdate = false)
            : base(name, validationFunc, validationExceptionMessage, alwaysIncludeInUpdate)
        {
        }

        /// <inheritdoc />
        public override void SaveChanges()
        {
            this.unmodifiedValue = this.currentValue;
        }

        /// <inheritdoc />
        public override void DiscardPendingChanges()
        {
            this.currentValue = this.unmodifiedValue;
        }
    }
}
