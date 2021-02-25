using HassClient.Models;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Intermediate object that contains all the objects that defines an <see cref="Entity"/>.
    /// </summary>
    public class EntityDefinition
    {
        /// <summary>
        /// Gets the entity domain.
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the entity initial state.
        /// </summary>
        public StateModel State { get; private set; }

        /// <summary>
        /// Gets the entity source.
        /// </summary>
        public EntitySource Source { get; private set; }

        /// <summary>
        /// Gets the entity service domain.
        /// </summary>
        public ServiceDomain ServiceDomain { get; private set; }

        /// <summary>
        /// Gets the entity registry entry.
        /// </summary>
        public EntityRegistryEntry EntityRegistryEntry { get; private set; }

        /// <summary>
        /// Gets the specific entity registry entry.
        /// </summary>
        public StorageEntityRegistryEntryBase SpecificEntityRegistryEntry { get; private set; }

        internal EntityDefinition(
            string domain,
            StateModel state,
            EntitySource source,
            ServiceDomain serviceDomain,
            EntityRegistryEntry entityRegistryEntry,
            StorageEntityRegistryEntryBase specificEntityRegistryEntry)
        {
            this.Domain = domain;
            this.State = state;
            this.Source = source;
            this.ServiceDomain = serviceDomain;
            this.EntityRegistryEntry = entityRegistryEntry;
            this.SpecificEntityRegistryEntry = specificEntityRegistryEntry;
        }
    }
}
