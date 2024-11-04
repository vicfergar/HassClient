using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a category.
    /// </summary>
    public class Category : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> scope = new ModifiableProperty<string>(nameof(Scope), alwaysIncludeInUpdate: true);

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the ID of this category.
        /// </summary>
        [JsonProperty(PropertyName = "category_id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the scope of this category.
        /// </summary>
        [JsonProperty]
        public string Scope
        {
            get => this.scope.Value;
            set => this.scope.Value = value;
        }

        [JsonConstructor]
        private Category()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="icon">The icon to display in front of the category in the front-end.</param>
        /// <param name="scope">The scope of the category.</param>
        public Category(string name, string scope, string icon = null)
            : base(name, icon)
        {
            this.Scope = scope;
        }

        // Used for testing purposes.
        internal static Category CreateUnmodified(string name, string icon, string scope)
        {
            var result = new Category(name, icon, scope);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties()
                       .Append(this.scope);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Category)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Category category &&
                   this.Id == category.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 1270127369 + EqualityComparer<string>.Default.GetHashCode(this.Id);
        }

        // Used for testing purposes.
        internal Category Clone()
        {
            var result = CreateUnmodified(this.Name, this.Scope, this.Icon);
            result.UniqueId = this.UniqueId;
            return result;
        }
    }
}
