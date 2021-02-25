using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HassClient.Entities.Collections
{
    /// <summary>
    /// Represents a collection of values with observable capabilities.
    /// </summary>
    /// <typeparam name="TValue">The type of the collection values.</typeparam>
    public interface IReadOnlyObservableCollection<TValue>
        : IReadOnlyCollection<TValue>,
          INotifyCollectionChanged,
          INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the value associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the value to get.</param>
        /// <returns>The value if exists, otherwise; its default value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        TValue FindById(string id);

        /// <summary>
        /// Gets the value associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the value to get.</param>
        /// <typeparam name="T">A type that inherits from <typeparamref name="TValue"/>.</typeparam>
        /// <returns>The value as <typeparamref name="T"/> if exists, otherwise; its default value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        T FindById<T>(string id)
            where T : class, TValue;

        /// <summary>
        /// Gets the value associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified id, if the id is found;
        /// otherwise, the default value for the <typeparamref name="TValue"/> of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="IReadOnlyObservableCollection{TValue}"/> contains an element with the
        /// specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        bool TryFindById(string id, out TValue value);

        /// <summary>
        /// Gets the value associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified id, if the id is found;
        /// otherwise, the default value for the <typeparamref name="TValue"/> of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <typeparam name="T">A type that inherits from <typeparamref name="TValue"/>.</typeparam>
        /// <returns>
        /// <c>true</c> if the <see cref="IReadOnlyObservableCollection{TValue}"/> contains an element with the
        /// specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        bool TryFindById<T>(string id, out T value)
            where T : class, TValue;
    }
}
