using HassClient.Helpers;
using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Factory used to create Storage Collection Messages.
    /// </summary>
    /// <typeparam name="TStorageEntity">
    /// The <see cref="StorageEntityRegistryEntryBase"/> type associated with the Storage Collection.
    /// </typeparam>
    public class StorageCollectionMessagesFactory<TStorageEntity>
        : RegistryEntryCollectionMessagesFactory<TStorageEntity>
        where TStorageEntity : StorageEntityRegistryEntryBase
    {
        /* TODO: Implement other found API prefixes:
             * "tag"
             * "image"
             * "counter"
             * "input_number"
             * "input_select"
             * "input_text
             * "zone"
             * "timer"
             * "input_datetime"
             */

        /// <summary>
        /// Creates a <see cref="StorageCollectionMessagesFactory{TStorageEntity}"/> for the given model type.
        /// </summary>
        /// <returns>
        /// A <see cref="StorageCollectionMessagesFactory{TStorageEntity}"/> for the given model type.
        /// </returns>
        public static StorageCollectionMessagesFactory<TStorageEntity> Create()
        {
            var domain = StorageEntityRegistryEntryBase.GetDomain<TStorageEntity>();
            return new StorageCollectionMessagesFactory<TStorageEntity>(domain);
        }

        private StorageCollectionMessagesFactory(KnownDomains domain)
            : base(domain.ToDomainString(), domain.ToDomainString())
        {
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to add a new registry entry in the storage collection.
        /// </summary>
        /// <param name="entry">The storage collection entry.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new registry entry in the storage collection.
        /// </returns>
        public new BaseOutgoingMessage CreateCreateMessage(TStorageEntity entry)
        {
            return base.CreateCreateMessage(entry);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to update an existing registry entry in the storage collection.
        /// </summary>
        /// <param name="entry">The storage collection entry.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property. If the entity does not
        /// support partial updates, this parameter is ignored.
        /// </param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used update an existing registry entry in the storage collection.
        /// </returns>
        public new BaseOutgoingMessage CreateUpdateMessage(TStorageEntity entry, bool forceUpdate)
        {
            var shouldForceUpdate = !entry.SupportsPartialUpdates || forceUpdate;
            return base.CreateUpdateMessage(entry, shouldForceUpdate);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to delete an existing registry entry from the storage collection.
        /// </summary>
        /// <param name="entry">The storage collection entry.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used delete an existing registry entry from the storage collection.
        /// </returns>
        public new BaseOutgoingMessage CreateDeleteMessage(TStorageEntity entry)
        {
            return base.CreateDeleteMessage(entry);
        }
    }
}
