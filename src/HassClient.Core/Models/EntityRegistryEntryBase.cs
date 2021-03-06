using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Base class that defines a entity registry entry.
    /// </summary>
    public abstract class EntityRegistryEntryBase : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> name = new ModifiableProperty<string>(nameof(Name));

        private readonly ModifiableProperty<string> icon = new ModifiableProperty<string>(nameof(Icon));

        /// <summary>
        /// Gets a value indicating that the name of the entity registry entry can be
        /// <see langword="null"/> or whitespace. It is <see langword="false"/> by default.
        /// </summary>
        protected virtual bool AcceptsNullOrWhiteSpaceName => false;

        /// <summary>
        /// Gets the entity identifier of the entity.
        /// </summary>
        [JsonIgnore]
        public abstract string EntityId { get; }

        /// <summary>
        /// Gets or sets the friendly name of this entity.
        /// </summary>
        public string Name
        {
            get => this.name.Value;
            set
            {
                if (!this.AcceptsNullOrWhiteSpaceName &&
                    string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"'{nameof(this.Name)}' cannot be null or whitespace.");
                }

                this.name.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon to display in front of the entity in the front-end.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Icon
        {
            get => this.icon.Value;
            set => this.icon.Value = value;
        }

        [JsonConstructor]
        private protected EntityRegistryEntryBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRegistryEntryBase"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        protected EntityRegistryEntryBase(string name, string icon)
        {
            if (!this.AcceptsNullOrWhiteSpaceName &&
                string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Icon = icon;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return this.name;
            yield return this.icon;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is EntityRegistryEntryBase registryEntryBase &&
                   this.UniqueId == registryEntryBase.UniqueId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.UniqueId);
        }
    }
}
