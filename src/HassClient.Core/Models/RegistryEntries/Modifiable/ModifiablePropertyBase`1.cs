using System;

namespace HassClient.Models
{
    /// <summary>
    /// Base class that represents a modifiable property from a model.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    public abstract class ModifiablePropertyBase<T> : IModifiableProperty
    {
        private Func<T, bool> validationFunc;

        private readonly string validationExceptionMessage;

        /// <inheritdoc />
        public string Name
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public abstract bool HasPendingChanges { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiablePropertyBase{T}"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        public ModifiablePropertyBase(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiablePropertyBase{T}"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="validationFunc">
        /// A function to validate new values for this property.
        /// </param>
        /// <param name="validationExceptionMessage">
        /// An error message used for the exception thew when validation fails.
        /// </param>
        public ModifiablePropertyBase(string name, Func<T, bool> validationFunc, string validationExceptionMessage = null)
            : this(name)
        {
            this.validationFunc = validationFunc;
            this.validationExceptionMessage = validationExceptionMessage;
        }

        /// <summary>
        /// Validates the given <paramref name="value"/> when a validation function is specified for this property.
        /// </summary>
        /// <param name="value">The value to test.</param>
        protected void ValidateValue(T value)
        {
            if (this.validationFunc?.Invoke(value) == false)
            {
                var message = this.validationExceptionMessage ?? $"'{value}' is not valid value for the property {this.Name}.";
                throw new InvalidOperationException(message);
            }
        }

        /// <inheritdoc />
        public abstract void SaveChanges();

        /// <inheritdoc />
        public abstract void DiscardPendingChanges();
    }
}
