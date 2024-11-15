using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing devices in Home Assistant.
    /// </summary>
    public class DevicesApi : ResourceApi
    {
        internal DevicesApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="Device"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Device"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<Device>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = DeviceRegistryMessagesFactory.Instance.BuildListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Device>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Updates an existing <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> with the new values.</param>
        /// <param name="disable">If not <see langword="null"/>, it will enable or disable the entity.</param>
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
        public async Task<bool> UpdateAsync(Device device, bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = DeviceRegistryMessagesFactory.Instance.BuildUpdateMessage(device, disable, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(device);
            }

            return result.Success;
        }
    }
}
