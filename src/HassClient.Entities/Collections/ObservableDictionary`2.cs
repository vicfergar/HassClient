using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HassClient.Entities.Collections
{
    /// <summary>
    /// Represents a dynamic data dictionary that provides notifications when items get added, removed,
    /// or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ObservableDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>,
        IDictionary<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        /// <inheritdoc />
        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                TValue existing;
                if (this.TryGetValue(key, out existing))
                {
                    base[key] = value;

                    this.CollectionChanged?.Invoke(
                        this,
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Replace,
                            new KeyValuePair<TKey, TValue>(key, value),
                            new KeyValuePair<TKey, TValue>(key, existing)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Values)));
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Occurs raised when a property on the collection changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, TKey key, TValue value)
        {
            if (this.CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs args;
                if (action == NotifyCollectionChangedAction.Reset)
                {
                    args = new NotifyCollectionChangedEventArgs(action);
                }
                else
                {
                    var pair = new KeyValuePair<TKey, TValue>(key, value);
                    args = new NotifyCollectionChangedEventArgs(action, pair);
                }

                this.CollectionChanged(this, args);
            }

            if (action != NotifyCollectionChangedAction.Replace)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Count)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Keys)));
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Values)));
        }

        /// <summary>
        /// Allows derived classes to raise custom property changed events.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }

        /// <inheritdoc />
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            this.RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, key, value);
        }

        /// <inheritdoc />
        public new void Clear()
        {
            base.Clear();
            this.RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Reset, default, default);
        }

        /// <inheritdoc />
        public new bool Remove(TKey key) => this.Remove(key, out _);

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. It can be null.</param>
        /// <returns>
        /// <see langword="true"/> if the key/value pair was added to the dictionary successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public new bool TryAdd(TKey key, TValue value)
        {
            if (base.TryAdd(key, value))
            {
                this.RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, key, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="ObservableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The removed value.</param>
        /// <returns>
        /// <see langword="true"/> if the element is successfully found and removed;
        /// otherwise, <see langword="false"/>.
        /// This method returns false if key is not found in the <see cref="ObservableDictionary{TKey, TValue}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public new bool Remove(TKey key, out TValue value)
        {
            if (base.Remove(key, out value))
            {
                this.RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, key, value);
                return true;
            }

            return false;
        }
    }
}
