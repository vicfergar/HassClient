using HassClient.Helpers;
using HassClient.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.PersistentNotification"/> domain.
    /// </summary>
    /// <remarks>
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/persistent_notification/"/>.
    /// </remarks>
    public sealed class PersistentNotificationEntity : Entity
    {
        /// <summary>
        /// Gets the notification ID.
        /// </summary>
        public string NotificationId => this.EntityId?.SplitEntityId().LastOrDefault();

        /// <summary>
        /// Gets the optional title of the notification.
        /// </summary>
        public string Title => this.State.GetAttributeValue<string>("title");

        /// <summary>
        /// Gets the message body of the notification.
        /// </summary>
        public string Message => this.State.GetAttributeValue<string>("message");

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentNotificationEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        internal PersistentNotificationEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        private Task<bool> CallServiceAsync(KnownServices service, CancellationToken cancellationToken)
        {
            return this.hassInstance.HassWSApi.CallServiceAsync(this.KnownDomain, service, data: new { this.NotificationId }, cancellationToken);
        }

        /// <summary>
        /// Remove the notification from the frontend.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> DismissAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Dismiss, cancellationToken);
        }

        /// <summary>
        /// Mark the notification read.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> MarkReadAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.MarkRead, cancellationToken);
        }
    }
}
