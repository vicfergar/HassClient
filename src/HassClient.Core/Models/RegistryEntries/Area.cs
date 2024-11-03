using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area : RegistryEntryBase, IAliasable, ILabelable
    {
        private readonly AliasesModifiableProperty aliases = new AliasesModifiableProperty();

        private readonly LabelsModifiableProperty labels = new LabelsModifiableProperty();

        private readonly ModifiableProperty<string> picture = new ModifiableProperty<string>(nameof(Picture));

        private readonly ModifiableProperty<string> floorId = new ModifiableProperty<string>(nameof(FloorId));

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <inheritdoc />
        public ICollection<string> Aliases => this.aliases.Value;

        /// <inheritdoc />
        public ICollection<string> Labels => this.labels.Value;

        /// <summary>
        /// Gets the ID of this area.
        /// </summary>
        [JsonProperty(PropertyName = "area_id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets a URL (relative or absolute) to a picture for this area.
        /// </summary>
        [JsonProperty]
        public string Picture
        {
            get => this.picture.Value;
            set => this.picture.Value = value;
        }

        /// <summary>
        /// Gets or sets the ID of the floor this area is on.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FloorId
        {
            get => this.floorId.Value;
            set => this.floorId.Value = value;
        }

        [JsonConstructor]
        private Area()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Area"/> class.
        /// </summary>
        /// <param name="name">The name of the area.</param>
        /// <param name="icon">The icon to display in front of the area in the front-end.</param>
        /// <param name="picture">a URL (relative or absolute) to a picture for this area.</param>
        /// <param name="floorId">The ID of the floor this area is on.</param>
        /// <param name="aliases">The aliases for this area.</param>
        /// <param name="labels">The labels for this area.</param>
        public Area(string name, string icon = null, string picture = null, string floorId = null, IEnumerable<string> aliases = null, IEnumerable<string> labels = null)
            : base(name, icon)
        {
            this.Picture = picture;
            this.FloorId = floorId;

            if (aliases != null)
            {
                this.aliases.AddRange(aliases);
            }

            if (labels != null)
            {
                this.labels.AddRange(labels);
            }
        }

        // Used for testing purposes.
        internal static Area CreateUnmodified(string name, string icon, string picture, string floorId, IEnumerable<string> aliases = null, IEnumerable<string> labels = null)
        {
            var result = new Area(name, icon, picture, floorId, aliases, labels);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties()
                       .Append(this.picture)
                       .Append(this.floorId)
                       .Append(this.aliases)
                       .Append(this.labels);
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
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(this.Id);
        }

        // Used for testing purposes.
        internal Area Clone()
        {
            var result = CreateUnmodified(this.Name, this.Icon, this.Picture, this.FloorId, this.Aliases, this.Labels);
            result.UniqueId = this.UniqueId;
            return result;
        }
    }
}
