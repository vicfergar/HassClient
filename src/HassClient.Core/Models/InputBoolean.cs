using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    public class InputBoolean : EntityRegistryEntryBase
    {
        private readonly ModifiableProperty<bool> initial = new ModifiableProperty<bool>(nameof(Initial));

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the entity identifier of the <see cref="InputBoolean"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the initial value when Home Assistant starts.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Initial
        {
            get => this.initial.Value;
            set => this.initial.Value = value;
        }

        /// <inheritdoc />
        public override string EntityId => $"input_boolean.{this.UniqueId}";

        [JsonConstructor]
        private InputBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBoolean"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        /// <param name="initial">The  initial value when Home Assistant starts.</param>
        public InputBoolean(string name, string icon = null, bool initial = false)
            : base(name, icon)
        {
            this.Initial = initial;
        }

        // Used for testing purposes.
        internal static InputBoolean CreateUnmodified(string uniqueId, string name, string icon = null, bool initial = false)
        {
            var result = new InputBoolean(name, icon, initial) { Id = uniqueId };
            result.SaveChanges();
            return result;
        }

        /// <inheritdoc />
        protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
        {
            return base.GetModifiableProperties().Append(this.initial);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(InputBoolean)}: {this.Name}";

        // Used for testing purposes.
        internal InputBoolean Clone()
        {
            var result = CreateUnmodified(this.UniqueId, this.Name, this.Icon, this.Initial);
            return result;
        }
    }
}
