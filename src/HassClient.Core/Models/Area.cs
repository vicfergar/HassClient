using Newtonsoft.Json;
using System;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area : ModifiableModelBase<Area>
    {
        private string name;

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
            get => this.name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException($"{nameof(this.Name)} cannot be null or white space.");
                }

                this.name = value;
            }
        }

        // Needed for serialization.
        private Area()
        {
        }

        // Needed for testing.
        internal Area(string name)
        {
            this.Name = name;
            this.ClearPendingChanges();
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

        /// <inheritdoc />
        protected override int GetModificationHash()
        {
            return HashCode.Combine(this.name);
        }

        /// <inheritdoc />
        protected internal override void Update(Area updatedModel)
        {
            this.Name = updatedModel.Name;
            base.Update(updatedModel);
        }
    }
}
