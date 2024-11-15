using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing areas in Home Assistant.
    /// </summary>
    public class AreasApi : ResourceApi
    {
        internal AreasApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="Area"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Area"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<Area>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.BuildListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Area>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> with the new values.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync(Area area, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.BuildCreateMessage(area);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(area);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> with the new values.</param>
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
        public async Task<bool> UpdateAsync(Area area, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.BuildUpdateMessage(area, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(area);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(Area area, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.BuildDeleteMessage(area);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                area.Untrack();
            }

            return success;
        }
    }
}
