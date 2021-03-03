using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HassClient.Models
{
    /// <summary>
    /// Defines a model that can be updated by the user using the API.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class ModifiableModelBase<T>
    {
        private int lastUpdateHash;

        /// <summary>
        /// Gets a value indicating that the model has pending changes waiting to update.
        /// </summary>
        [JsonIgnore]
        public bool HasPendingChanges => this.lastUpdateHash != this.GetModificationHash();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ClearPendingChanges();
        }

        /// <summary>
        /// Clears the <see cref="HasPendingChanges"/> property.
        /// </summary>
        protected void ClearPendingChanges()
        {
            this.lastUpdateHash = this.GetModificationHash();
        }

        /// <summary>
        /// Gets a hash value that represents all modifiable properties.
        /// </summary>
        /// <returns>An integer value that represents all modifiable properties.</returns>
        protected abstract int GetModificationHash();

        /// <summary>
        /// Called internally by the API client when model is updated by user.
        /// </summary>
        /// <param name="updatedModel">The received updated model.</param>
        protected internal virtual void Update(T updatedModel)
        {
            this.ClearPendingChanges();
        }
    }
}
