using Newtonsoft.Json;
using System;

namespace HassClient.Models
{
    /// <summary>
    /// Base class that defines a registry entry.
    /// </summary>
    public abstract class RegistryEntryBase : ModifiableModelBase<RegistryEntryBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryEntryBase"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        protected RegistryEntryBase(string name, string icon)
        {
            this.Name = name;
            this.Icon = icon;
            this.ClearPendingChanges();
        }

        /// <summary>
        /// Gets the entity identifier of the entity.
        /// </summary>
        [JsonIgnore]
        public abstract string EntityId { get; }

        /// <summary>
        /// Gets or sets the friendly name of this entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon to display in front of the entity in the front-end.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// Gets the unique identifier of this entity.
        /// </summary>
        public abstract string UniqueId { get; internal set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is RegistryEntryBase registryEntryBase &&
                   this.UniqueId == registryEntryBase.UniqueId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.UniqueId);
        }

        /// <inheritdoc />
        protected override int GetModificationHash()
        {
            return HashCode.Combine(this.Name, this.Icon);
        }

        /// <inheritdoc />
        protected internal override void Update(RegistryEntryBase updatedModel)
        {
            this.Name = updatedModel.Name;
            this.Icon = updatedModel.Icon;

            base.Update(updatedModel);
        }
    }
}
