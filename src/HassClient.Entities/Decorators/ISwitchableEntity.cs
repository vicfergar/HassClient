using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity that can be switched between on and off states.
    /// </summary>
    public interface ISwitchableEntity
    {
        /// <summary>
        /// Gets a value indicating whether the switchable entity is on.
        /// </summary>
        bool IsOn { get; }

        /// <summary>
        /// Turns on the switchable entity.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        Task<bool> ToggleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Turns off the switchable entity.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        Task<bool> TurnOffAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Turns on the switchable entity.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        Task<bool> TurnOnAsync(CancellationToken cancellationToken = default);
    }
}
