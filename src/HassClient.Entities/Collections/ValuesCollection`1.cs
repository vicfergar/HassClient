using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HassClient.Entities.Collections
{
    /// <summary>
    /// Represents a collection of values with observable capabilities.
    /// </summary>
    /// <typeparam name="TValue">The type of the collection values.</typeparam>
    public class ValuesCollection<TValue> : IReadOnlyObservableCollection<TValue>
    {
        private ObservableDictionary<string, TValue> dictionary;

        internal ValuesCollection(ObservableDictionary<string, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <inheritdoc />
        public int Count => this.dictionary.Count;

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => this.dictionary.CollectionChanged += value;
            remove => this.dictionary.CollectionChanged -= value;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => this.dictionary.PropertyChanged += value;
            remove => this.dictionary.PropertyChanged -= value;
        }

        /// <inheritdoc />
        public TValue FindById(string id)
        {
            if (this.dictionary.TryGetValue(id, out var value))
            {
                return value;
            }

            return default;
        }

        /// <inheritdoc />
        public T FindById<T>(string id)
            where T : class, TValue
        {
            return this.FindById(id) as T;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var item in this.dictionary.Values)
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public bool TryFindById(string id, out TValue value)
        {
            return this.dictionary.TryGetValue(id, out value);
        }

        /// <inheritdoc />
        public bool TryFindById<T>(string id, out T value)
            where T : class, TValue
        {
            if (this.TryFindById(id, out var result))
            {
                value = result as T;
            }
            else
            {
                value = default;
            }

            return value != default;
        }
    }
}
