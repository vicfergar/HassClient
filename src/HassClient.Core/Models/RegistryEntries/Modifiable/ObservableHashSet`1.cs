using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an observable set that can track pending changes.
    /// </summary>
    /// <typeparam name="T">The collection elements type.</typeparam>
    internal class ObservableHashSet<T> : ObservableCollection<T>
    {
        private HashSet<T> addedValues = new HashSet<T>();

        private HashSet<T> removedValues = new HashSet<T>();

        private Action<T> validationCallback;

        public bool HasPendingChanges => this.addedValues.Count > 0 || this.removedValues.Count > 0;

        public ObservableHashSet(Action<T> validationCallback)
        {
            this.validationCallback = validationCallback;
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item))
            {
                return;
            }

            this.validationCallback(item);
            if (!this.removedValues.Remove(item))
            {
                this.addedValues.Add(item);
            }

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            var item = this[index];

            if (!this.addedValues.Remove(item))
            {
                this.removedValues.Add(item);
            }

            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (var item in this.Except(this.addedValues))
            {
                this.removedValues.Add(item);
            }

            this.addedValues.Clear();

            base.ClearItems();
        }

        /// <inheritdoc />
        protected override void SetItem(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            this.addedValues.Clear();
            this.removedValues.Clear();
        }

        public void DiscardPendingChanges()
        {
            foreach (var item in this.removedValues)
            {
                base.InsertItem(this.Count, item);
            }

            foreach (var item in this.addedValues)
            {
                var index = this.IndexOf(item);
                base.RemoveItem(index);
            }

            this.addedValues.Clear();
            this.removedValues.Clear();
        }
    }
}
