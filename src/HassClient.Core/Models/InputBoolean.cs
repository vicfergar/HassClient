using Newtonsoft.Json;
using System;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    public class InputBoolean : RegistryEntryBase
    {
        /// <inheritdoc />
        [JsonProperty("id")]
        public override string UniqueId { get; internal set; }

        /// <summary>
        /// Gets or sets the initial value when Home Assistant starts.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Initial { get; set; }

        /// <inheritdoc />
        public override string EntityId => $"input_boolean.{this.UniqueId}";

        /// <inheritdoc />
        public override string ToString() => $"{nameof(InputBoolean)}: {this.Name}";

        private InputBoolean()
            : base(null, null)
        {
        }

        internal InputBoolean(string name, string icon = null, bool initial = false)
            : base(name, icon)
        {
            this.Initial = initial;
            this.ClearPendingChanges();
        }

        /// <inheritdoc />
        protected override int GetModificationHash()
        {
            return HashCode.Combine(this.Initial, base.GetModificationHash());
        }

        /// <inheritdoc />
        protected internal override void Update(RegistryEntryBase updatedModel)
        {
            this.Initial = ((InputBoolean)updatedModel).Initial;
            base.Update(updatedModel);
        }
    }
}
