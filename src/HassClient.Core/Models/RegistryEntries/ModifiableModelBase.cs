using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HassClient.Models
{
    /// <summary>
    /// Defines a registry entry model that can be updated by the user using the API.
    /// </summary>
    public abstract class ModifiableModelBase
    {
        private IModifiableProperty[] modifiableProperties;

        /// <summary>
        /// Gets a value indicating whether the object has been deserialized.
        /// </summary>
        protected bool isDeserialized;

        /// <summary>
        /// Gets the unique identifier that represents this Registry Entry.
        /// </summary>
        internal protected abstract string UniqueId { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity supports partial updates.
        /// </summary>
        [JsonIgnore]
        public virtual bool SupportsPartialUpdates => true;

        /// <summary>
        /// Gets a value indicating that the registry entry already exists on the Home Assistant instance.
        /// </summary>
        [JsonIgnore]
        public bool IsTracked => this.isDeserialized && this.UniqueId != null;

        /// <summary>
        /// Gets a value indicating that the registry entry is marked as dirty and is pending to be updated.
        /// </summary>
        [JsonIgnore]
        public bool IsDirty { get; internal set; }

        /// <summary>
        /// Gets a value indicating that the model has pending changes waiting to update.
        /// </summary>
        [JsonIgnore]
        public bool HasPendingChanges => this.modifiableProperties.Any(x => x.HasPendingChanges);

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableModelBase"/> class.
        /// </summary>
        public ModifiableModelBase()
        {
            this.modifiableProperties = this.GetModifiableProperties().ToArray();
        }

        /// <summary>
        /// Gets all modifiable properties of the model.
        /// </summary>
        /// <returns>The modifiable properties of the model.</returns>
        protected abstract IEnumerable<IModifiableProperty> GetModifiableProperties();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.isDeserialized = true;
            this.SaveChanges();
        }

        /// <summary>
        /// Clears the <see cref="HasPendingChanges"/> property.
        /// <para>
        /// Called internally when the model is deserialized or populated with updated values.
        /// </para>
        /// </summary>
        protected void SaveChanges()
        {
            foreach (var property in this.modifiableProperties)
            {
                property.SaveChanges();
            }
        }

        /// <summary>
        /// Discard any pending changes made on the entity and clears the <see cref="HasPendingChanges"/> property.
        /// </summary>
        public void DiscardPendingChanges()
        {
            foreach (var property in this.modifiableProperties)
            {
                property.DiscardPendingChanges();
            }
        }

        internal void Untrack()
        {
            this.UniqueId = null;
        }

        internal IEnumerable<string> GetModifiablePropertyNames()
        {
            return this.modifiableProperties
                          .Select(x => x.Name);
        }

        internal IEnumerable<string> GetModifiedPropertyNames()
        {
            return this.modifiableProperties
                       .Where(x => x.HasPendingChanges || x.AlwaysIncludeInUpdate)
                       .Select(x => x.Name);
        }
    }
}
