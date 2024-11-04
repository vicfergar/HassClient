using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a label.
    /// </summary>
    public class Label : RegistryEntryBase
    {
        private readonly ModifiableProperty<string> color = new ModifiableProperty<string>(nameof(Color));

        private readonly ModifiableProperty<string> description = new ModifiableProperty<string>(nameof(Description));

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the ID of this floor.
        /// </summary>
        [JsonProperty(PropertyName = "label_id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the color of the label.
        /// </summary>
        [JsonProperty]
        public string Color
        {
            get => this.color.Value;
            set => this.color.Value = value;
        }

        /// <summary>
        /// Gets or sets the description of the label.
        /// </summary>
        [JsonProperty]
        public string Description
        {
            get => this.description.Value;
            set => this.description.Value = value;
        }

        [JsonConstructor]
        private Label()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="name">The name of the label.</param>
        /// <param name="icon">The icon to display in front of the label in the front-end.</param>
        /// <param name="color">The color of the label.</param>
        /// <param name="description">The description of the label.</param>
        public Label(string name, string icon = null, string color = null, string description = null)
            : base(name, icon)
        {
            this.Color = color;
            this.Description = description;
        }

        // Used for testing purposes.
        internal static Label CreateUnmodified(string name, string icon, string color, string description)
        {
            var result = new Label(name, icon, color, description);
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties()
                       .Append(this.color)
                       .Append(this.description);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Label)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Label label &&
                   this.Id == label.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id);
        }

        // Used for testing purposes.
        internal Label Clone()
        {
            var result = CreateUnmodified(this.Name, this.Icon, this.Color, this.Description);
            result.UniqueId = this.UniqueId;
            return result;
        }
    }
}
