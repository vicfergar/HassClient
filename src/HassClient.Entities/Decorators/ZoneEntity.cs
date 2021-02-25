using HassClient.Models;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Zone"/> domain.
    /// </summary>
    /// <remarks>
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/zone"/>.
    /// </remarks>
    public class ZoneEntity :
        CommitableEntity<Zone>
    {
        /// <summary>
        /// Gets or sets the latitude of the center point of the zone.
        /// </summary>
        public float Latitude
        {
            get => this.GetPropertyOrFallbackAttribute("latitude", (x) => x.Latitude);
            set => this.SpecificEntityRegistryEntry.Latitude = value;
        }

        /// <summary>
        /// Gets or sets the longitude of the center point of the zone.
        /// </summary>
        public float Longitude
        {
            get => this.GetPropertyOrFallbackAttribute("longitude", (x) => x.Longitude);
            set => this.SpecificEntityRegistryEntry.Longitude = value;
        }

        /// <summary>
        /// Gets or sets the radius of the zone in meters.
        /// </summary>
        public float Radius
        {
            get => this.GetPropertyOrFallbackAttribute("radius", (x) => x.Radius);
            set => this.SpecificEntityRegistryEntry.Radius = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the zone will be used only for automation and hide it
        /// from the frontend and not use the zone for device tracker name.
        /// </summary>
        public bool IsPassive
        {
            get => this.GetPropertyOrFallbackAttribute("passive", (x) => x.IsPassive);
            set => this.SpecificEntityRegistryEntry.IsPassive = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal ZoneEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }
    }
}
