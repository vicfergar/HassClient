using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a floor.
    /// </summary>
    public class Floor : RegistryEntryBase, IAliasable
    {
        private readonly AliasesModifiableProperty aliases = new AliasesModifiableProperty();

        private readonly ModifiableProperty<int> level = new ModifiableProperty<int>(nameof(Level));

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <inheritdoc />
        public ICollection<string> Aliases => this.aliases.Value;

        /// <summary>
        /// Gets the ID of this floor.
        /// </summary>
        [JsonProperty(PropertyName = "floor_id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the level of the floor.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Level
        {
            get => this.level.Value;
            set => this.level.Value = value;
        }

        [JsonConstructor]
        private Floor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Floor"/> class.
        /// </summary>
        /// <param name="name">The name of the floor.</param>
        /// <param name="icon">The icon to display in front of the floor in the front-end.</param>
        /// <param name="level">The level of the floor.</param>
        /// <param name="aliases">The aliases associated with this floor.</param>
        public Floor(string name, string icon = null, int? level = null, IEnumerable<string> aliases = null)
            : base(name, icon)
        {
            this.Level = level ?? 0;

            if (aliases != null)
            {
                this.aliases.AddRange(aliases);
            }
        }

        // Used for testing purposes.
        internal static Floor CreateUnmodified(string name, string icon, int level, IEnumerable<string> aliases = null)
        {
            var result = new Floor(name, icon, level, aliases);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties()
                       .Append(this.level)
                       .Append(this.aliases);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Floor)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Floor floor &&
                   this.Id == floor.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 1597463007 + EqualityComparer<string>.Default.GetHashCode(this.Id);
        }

        // Used for testing purposes.
        internal Floor Clone()
        {
            var result = CreateUnmodified(this.Name, this.Icon, this.Level, this.Aliases);
            result.UniqueId = this.UniqueId;
            return result;
        }
    }
}
