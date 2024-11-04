using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a modifiable property from a model.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1648:inheritdoc should be used with inheriting class", Justification = "Inherits document from base constructor")]
    public class ModifiablePropertyCollection<T> : ModifiablePropertyBase<T>
    {
        private ObservableHashSet<T> currentValues;

        /// <summary>
        /// Gets property collection values.
        /// </summary>
        public ICollection<T> Value
        {
            get => this.currentValues;
        }

        /// <inheritdoc />
        public override bool HasPendingChanges => this.currentValues.HasPendingChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiablePropertyCollection{T}"/> class.
        /// </summary>
        /// <inheritdoc/>
        public ModifiablePropertyCollection(string name)
            : this(name, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiablePropertyCollection{T}"/> class.
        /// </summary>
        /// <inheritdoc/>
        public ModifiablePropertyCollection(string name, Func<T, bool> validationFunc, string validationExceptionMessage = null)
            : base(name, validationFunc, validationExceptionMessage)
        {
            this.currentValues = new ObservableHashSet<T>(this.ValidateValue);
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.currentValues.Add(item);
            }
        }

        /// <inheritdoc />
        public override void SaveChanges()
        {
            this.currentValues.SaveChanges();
        }

        /// <inheritdoc />
        public override void DiscardPendingChanges()
        {
            this.currentValues.DiscardPendingChanges();
        }
    }
}
