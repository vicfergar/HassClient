using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing storage entity registry entries in Home Assistant.
    /// </summary>
    public class StorageEntitiesApi : ResourceApi
    {
        internal StorageEntitiesApi(HassClientWebSocket webSocket)
         : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered storage entity registry entry of the given type
        /// in the Home Assistant instance.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <typeparamref name="TStorageEntity"/> entity in the Home Assistant instance.
        /// </returns>
        public async Task<IEnumerable<TStorageEntity>> ListAsync<TStorageEntity>(CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateListMessage();
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                if (typeof(TStorageEntity) == typeof(Person))
                {
                    var response = result.DeserializeResult<PersonResponse>();
                    return response.Storage
                                   .Select(person =>
                                   {
                                       person.IsStorageEntry = true;
                                       return person;
                                   })
                                   .Concat(response.Config)
                                   .Cast<TStorageEntity>();
                }

                return result.DeserializeResult<IEnumerable<TStorageEntity>>();
            }

            return null;
        }

        /// <summary>
        /// Creates a new storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The new storage entity registry entry.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateCreateMessage(storageEntity);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(storageEntity);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The storage entity registry entry with the updated values.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property. If the entity
        /// does not support partial updates, this parameter is ignored.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        public async Task<bool> UpdateAsync<TStorageEntity>(TStorageEntity storageEntity, bool forceUpdate = false, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateUpdateMessage(storageEntity, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(storageEntity);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The storage entity registry entry to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateDeleteMessage(storageEntity);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                storageEntity.Untrack();
            }

            return success;
        }
    }
}
