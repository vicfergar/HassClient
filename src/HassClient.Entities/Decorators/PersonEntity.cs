using HassClient.Models;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Person"/> domain.
    /// </summary>
    /// <remarks>
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/person"/>.
    /// </remarks>
    public class PersonEntity :
        CommitableEntity<Person>
    {
        /// <summary>
        /// Gets or sets the user account of the Home Assistant associated to this person entity.
        /// </summary>
        public User User
        {
            get => this.GetPropertyOrFallbackAttribute("user_id", (x) => x.UserId, (id) => this.hassInstance.Users.FindById(id));
            set => this.SpecificEntityRegistryEntry.ChangeUser(value);
        }

        /// <summary>
        /// Gets or sets a URL (relative or absolute) to a picture for the person entity.
        /// </summary>
        public string Picture
        {
            get => this.GetPropertyOrFallbackAttribute("entity_picture", (x) => x.Picture);
            set => this.SpecificEntityRegistryEntry.Picture = value;
        }

        /// <inheritdoc/>
        public override bool IsEditable => this.SpecificEntityRegistryEntry?.IsStorageEntry ?? false;

        /// <summary>
        /// Gets the <see cref="DeviceTrackerEntity"/> associated with this person entity that is currently active.
        /// </summary>
        public DeviceTrackerEntity ActiveTracker
        {
            get
            {
                var trackerId = this.State.GetAttributeValue<string>("source");
                return trackerId != null ? this.hassInstance.Entities.FindById(trackerId) as DeviceTrackerEntity : null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal PersonEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }
    }
}
