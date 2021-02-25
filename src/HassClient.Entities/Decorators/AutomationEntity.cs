using HassClient.Helpers;
using HassClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Automation"/> domain.
    /// </summary>
    /// <remarks>
    /// Developers documentation: <see href="https://developers.home-assistant.io/docs/device_automation_index"/>.
    /// Users documentation: <see href="https://www.home-assistant.io/docs/automation/"/>.
    /// </remarks>
    public class AutomationEntity
        : Entity,
        ISwitchableEntity,
        IReloadableEntity
    {
        private static KnownEnumCache<AutomationMode> automationModeCache = new KnownEnumCache<AutomationMode>();

        /// <inheritdoc/>
        public bool IsOn => this.State.KnownState == KnownStates.On;

        /// <summary>
        /// Gets a value indicating whether the is any running invocation of this automation.
        /// </summary>
        public bool IsRunning => this.Current > 0;

        /// <summary>
        /// The number of running invocations of this automation.
        /// </summary>
        public int Current => this.State.GetAttributeValue<int>("current");

        /// <summary>
        /// The id of this automation.
        /// </summary>
        public string Id => this.State.GetAttributeValue<string>("id");

        /// <summary>
        /// Gets the UTC date and time that this automation was last triggered.
        /// <para>
        /// When <see langword="null"/> it indicates that the automation has not
        /// yet been triggered since last Home Assistant startup.</para>
        /// </summary>
        public DateTimeOffset? LastTriggered => this.State.GetAttributeValue<DateTimeOffset?>("last_triggered");

        /// <summary>
        /// Gets the automation’s mode configuration option controls what happens when the
        /// automation is triggered while the actions are still running from a previous trigger.
        /// </summary>
        public AutomationMode Mode => this.State.GetAttributeValue("mode", automationModeCache);

        /// <summary>
        /// Gets the maximum number of runs that can be executing and/or queued up at a time.
        /// <para>
        /// This value is only available for both <see cref="AutomationMode.Queued"/> and
        /// <see cref="AutomationMode.Parallel"/> modes.
        /// </para>
        /// </summary>
        public int? Max => this.State.GetAttributeValue<int?>("max");

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal AutomationEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
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

        /// <inheritdoc/>
        public Task<bool> ReloadAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Reload, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Trigger the actions of an automation.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating
        /// if the action was successfully done.
        /// </returns>
        public Task<bool> Trigger(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Trigger, cancellationToken: cancellationToken);
        }
    }
}
