using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Base class that defines a entry with a name and an icon.
    /// </summary>
    public abstract class NamedEntryBase : ModifiableModelBase
    {
        private readonly ModifiableProperty<string> name = new ModifiableProperty<string>(nameof(Name));

        private readonly ModifiableProperty<string> icon = new ModifiableProperty<string>(nameof(Icon));

        /// <summary>
        /// Gets a value indicating that the name of the entity registry entry can be
        /// <see langword="null"/> or whitespace. It is <see langword="false"/> by default.
        /// </summary>
        protected virtual bool AcceptsNullOrWhiteSpaceName => false;

        /// <summary>
        /// Gets or sets the friendly name of this entity.
        /// </summary>
        public virtual string Name
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
        [JsonProperty]
        public virtual string Icon
        {
            get => this.icon.Value;
            set => this.icon.Value = value;
        }

        [JsonConstructor]
        private protected NamedEntryBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedEntryBase"/> class.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <param name="icon">The entry icon.</param>
        protected NamedEntryBase(string name, string icon)
        {
            if (!this.AcceptsNullOrWhiteSpaceName &&
                string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Icon = icon;
        }

        /// <summary>
        /// Method used by the serializer to determine if the <see cref="Icon"/> property should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the property should be serialized; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ShouldSerializeIcon() => this.Icon != null || this.IsTracked;

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return this.name;
            yield return this.icon;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is NamedEntryBase namedEntryBase &&
                   this.UniqueId == namedEntryBase.UniqueId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return -401120461 + EqualityComparer<string>.Default.GetHashCode(this.UniqueId);
        }
    }
}
