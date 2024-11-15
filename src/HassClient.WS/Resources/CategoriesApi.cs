using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an API for managing categories in Home Assistant.
    /// </summary>
    public class CategoriesApi : ResourceApi
    {
        internal CategoriesApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
        }

        /// <summary>
        /// Gets a collection with every registered <see cref="Category"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="scope">The scope of the categories to retrieve.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Category"/> in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<Category>> ListAsync(string scope, CancellationToken cancellationToken = default)
        {
            var commandMessage = CategoryRegistryMessagesFactory.Instance.CreateListMessage(scope);
            return this.HassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Category>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The <see cref="Category"/> with the new values.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            var commandMessage = CategoryRegistryMessagesFactory.Instance.CreateCreateMessage(category);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(category);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The <see cref="Category"/> with the new values.</param>
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
        public async Task<bool> UpdateAsync(Category category, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = CategoryRegistryMessagesFactory.Instance.CreateUpdateMessage(category, forceUpdate);
            var result = await this.HassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(category);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The <see cref="Category"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteAsync(Category category, CancellationToken cancellationToken = default)
        {
            var commandMessage = CategoryRegistryMessagesFactory.Instance.CreateDeleteMessage(category);
            var success = await this.HassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                category.Untrack();
            }

            return success;
        }
    }
}
