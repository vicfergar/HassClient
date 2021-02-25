using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity capable of reloading its internal configuration.
    /// </summary>
    public interface IReloadableEntity
    {
        /// <summary>
        /// Reloads the entity configuration.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating
        /// if the action was successfully done.
        /// </returns>
        Task<bool> ReloadAsync(CancellationToken cancellationToken = default);
    }
}
