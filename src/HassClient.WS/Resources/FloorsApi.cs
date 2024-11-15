using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing floors in Home Assistant.
    /// </summary>
    public class FloorsApi : ResourceApi
    {
        internal FloorsApi(HassClientWebSocket hassClientWebSocket)
            : base(hassClientWebSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="Floor"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Floor"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<Floor>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = FloorRegistryMessagesFactory.Instance.CreateListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Floor>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Floor"/>.
        /// </summary>
        /// <param name="floor">The <see cref="Floor"/> with the new values.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync(Floor floor, CancellationToken cancellationToken = default)
        {
            var commandMessage = FloorRegistryMessagesFactory.Instance.CreateCreateMessage(floor);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(floor);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing <see cref="Floor"/>.
        /// </summary>
        /// <param name="floor">The <see cref="Floor"/> with the new values.</param>
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
        public async Task<bool> UpdateAsync(Floor floor, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = FloorRegistryMessagesFactory.Instance.CreateUpdateMessage(floor, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(floor);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing <see cref="Floor"/>.
        /// </summary>
        /// <param name="floor">The <see cref="Floor"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(Floor floor, CancellationToken cancellationToken = default)
        {
            var commandMessage = FloorRegistryMessagesFactory.Instance.CreateDeleteMessage(floor);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                floor.Untrack();
            }

            return success;
        }
    }
}
