using HassClient.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.InputBoolean"/> domain.
    /// </summary>
    /// <remarks>
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/input_boolean"/>.
    /// </remarks>
    public class InputBooleanEntity :
        CommitableEntity<InputBoolean>,
        ISwitchableEntity
    {
        /// <inheritdoc/>
        public bool IsOn => this.State.KnownState == KnownStates.On;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBooleanEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal InputBooleanEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        /// <inheritdoc/>
        public Task<bool> TurnOnAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOn, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOffAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOff, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ToggleAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Toggle, cancellationToken: cancellationToken);
        }
    }
}
