namespace HassClient.Entities.Collections
{
    /// <summary>
    /// Represents a wrap over <see cref="ObservableDictionary{TKey, TValue}"/> with a view
    /// of the dictionary values as <see cref="IReadOnlyObservableCollection{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the collection values.</typeparam>
    public class HassCollection<TValue>
        : ObservableDictionary<string, TValue>
    {
        private ValuesCollection<TValue> values;

        /// <summary>
        /// Gets an <see cref="IReadOnlyObservableCollection{TValue}"/> containing the values in the
        /// <see cref="ObservableDictionary{TKey, TValue}"/>.
        /// </summary>
        public new IReadOnlyObservableCollection<TValue> Values => this.values;

        /// <summary>
        /// Initializes a new instance of the <see cref="HassCollection{TValue}"/> class.
        /// </summary>
        public HassCollection()
        {
            this.values = new ValuesCollection<TValue>(this);
        }
    }
}
