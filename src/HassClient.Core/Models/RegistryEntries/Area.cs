using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> name = new ModifiableProperty<string>(nameof(Name));

        private readonly ModifiableProperty<string> picture = new ModifiableProperty<string>(nameof(Picture));

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the ID of this area.
        /// </summary>
        [JsonProperty(PropertyName = "area_id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the name of this area.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get => this.name.Value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"'{nameof(this.Name)}' cannot be null or whitespace.");
                }

                this.name.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a URL (relative or absolute) to a picture for this area.
        /// </summary>
        [JsonProperty]
        public string Picture
        {
            get => this.picture.Value;
            set => this.picture.Value = value;
        }

        [JsonConstructor]
        private Area()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Area"/> class.
        /// </summary>
        /// <param name="name">The name of the area.</param>
        /// <param name="picture">a URL (relative or absolute) to a picture for this area.</param>
        public Area(string name, string picture = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Picture = picture;
        }

        // Used for testing purposes.
        internal static Area CreateUnmodified(string name, string picture)
        {
            var result = new Area(name, picture);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            yield return this.name;
            yield return this.picture;
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Area)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Area area &&
                   this.Id == area.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }

        // Used for testing purposes.
        internal Area Clone()
        {
            var result = CreateUnmodified(this.Name, this.Picture);
            result.UniqueId = this.UniqueId;
            return result;
        }
    }
}
