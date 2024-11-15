using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing labels in Home Assistant.
    /// </summary>
    public class LabelsApi : ResourceApi
    {
        internal LabelsApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="Label"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Label"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<Label>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = LabelRegistryMessagesFactory.Instance.CreateListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Label>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> with the new values.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync(Label label, CancellationToken cancellationToken = default)
        {
            var commandMessage = LabelRegistryMessagesFactory.Instance.CreateCreateMessage(label);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(label);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> with the new values.</param>
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
        public async Task<bool> UpdateAsync(Label label, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = LabelRegistryMessagesFactory.Instance.CreateUpdateMessage(label, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(label);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing <see cref="Label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(Label label, CancellationToken cancellationToken = default)
        {
            var commandMessage = LabelRegistryMessagesFactory.Instance.CreateDeleteMessage(label);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                label.Untrack();
            }

            return success;
        }
    }
}
