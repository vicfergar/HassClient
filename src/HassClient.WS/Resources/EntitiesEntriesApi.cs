using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing entity registry entries in Home Assistant.
    /// </summary>
    public class EntitiesEntriesApi : ResourceApi
    {
        internal EntitiesEntriesApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered entity in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered entity in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<EntityRegistryEntry>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<EntityRegistryEntry>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Gets a specific entity registry entry by its entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID to retrieve.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="EntityRegistryEntry"/>.
        /// </returns>
        public Task<EntityRegistryEntry> GetAsync(string entityId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException($"'{nameof(entityId)}' cannot be null or empty", nameof(entityId));
            }

            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
            return this.HassClientWebSocket.SendCommandWithResultAsync<EntityRegistryEntry>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Refresh a given <see cref="EntityRegistryEntry"/> with the values from the server.
        /// </summary>
        /// <param name="entityRegistryEntry">The entity registry entry to refresh.</param>
        /// <param name="newEntityId">If not <see langword="null"/>, it will be used as entity id.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// refresh operation was successfully done.
        /// </returns>
        public async Task<bool> RefreshAsync(EntityRegistryEntry entityRegistryEntry, string newEntityId = null, CancellationToken cancellationToken = default)
        {
            var entityId = newEntityId ?? entityRegistryEntry.EntityId;
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (!result.Success)
            {
                return false;
            }

            result.PopulateResult(entityRegistryEntry);
            return true;
        }

        /// <summary>
        /// Updates an existing entity registry entry.
        /// </summary>
        /// <param name="entity">The entity registry entry with the updated values.</param>
        /// <param name="newEntityId">If not null, it will update the current entity ID.</param>
        /// <param name="disable">If not null, it will enable or disable the entity.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        public async Task<bool> UpdateAsync(
            EntityRegistryEntry entity,
            string newEntityId = null,
            bool? disable = null,
            bool forceUpdate = false,
            CancellationToken cancellationToken = default)
        {
            if (newEntityId == entity.EntityId)
            {
                throw new ArgumentException($"{nameof(newEntityId)} cannot be the same as {nameof(entity.EntityId)}");
            }

            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateUpdateMessage(entity, newEntityId, disable, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync<EntityEntryResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.EntityEntryRaw, entity);
            return true;
        }

        /// <summary>
        /// Deletes an existing entity registry entry.
        /// </summary>
        /// <param name="entity">The entity registry entry to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(EntityRegistryEntry entity, CancellationToken cancellationToken = default)
        {
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateDeleteMessage(entity);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                entity.Untrack();
            }

            return success;
        }
    }
}
