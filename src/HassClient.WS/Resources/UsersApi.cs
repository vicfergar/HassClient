using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing users in Home Assistant.
    /// </summary>
    public class UsersApi : ResourceApi
    {
        internal UsersApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="User"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="User"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.BuildListMessage();
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<User>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="User"/>.
        /// </summary>
        /// <param name="user">The new <see cref="User"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.BuildCreateMessage(user);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.UserRaw, user);
            return true;
        }

        /// <summary>
        /// Updates an existing <see cref="User"/>.
        /// </summary>
        /// <param name="user">The <see cref="User"/> with the new values.</param>
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
        public async Task<bool> UpdateAsync(User user, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.BuildUpdateMessage(user, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.UserRaw, user);
            return true;
        }

        /// <summary>
        /// Deletes an existing <see cref="User"/>.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.BuildDeleteMessage(user);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                user.Untrack();
            }

            return success;
        }
    }
}
