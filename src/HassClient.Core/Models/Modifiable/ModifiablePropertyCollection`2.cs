using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a modifiable property from a model.
    /// </summary>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="T">The property type.</typeparam>
    public class ModifiablePropertyCollection<TCollection, T> : IModifiableProperty
        where TCollection : ICollection<T>
    {
        private bool isUnsaved = true;

        private TCollection unmodifiedValue;

        private TCollection currentValue;

        /// <summary>
        /// Gets or sets a value for the property.
        /// </summary>
        public TCollection Value
        {
            get => this.currentValue;
        }

        /// <inheritdoc />
        public string Name
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public bool HasPendingChange => this.isUnsaved || !this.AllElementsEqual();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiablePropertyCollection{TCollection, T}"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        public ModifiablePropertyCollection(string name)
        {
            this.Name = name;
            this.unmodifiedValue = Activator.CreateInstance<TCollection>();
            this.currentValue = Activator.CreateInstance<TCollection>();
        }

        private bool AllElementsEqual()
        {
            if (this.currentValue.Count() != this.unmodifiedValue.Count())
            {
                return false;
            }

            var c = this.currentValue.GetEnumerator();
            var u = this.unmodifiedValue.GetEnumerator();
            do
            {
                if (!Equals(c.Current, u.Current))
                {
                    return false;
                }
            }
            while (c.MoveNext() && u.MoveNext());

            return true;
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            this.unmodifiedValue.Clear();
            foreach (var item in this.currentValue)
            {
                this.unmodifiedValue.Add(item);
            }

            this.isUnsaved = false;
        }

        /// <inheritdoc />
        public void DiscardPendingChange()
        {
            this.currentValue.Clear();
            foreach (var item in this.unmodifiedValue)
            {
                this.currentValue.Add(item);
            }
        }
    }
}
