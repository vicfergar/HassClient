using HassClient.Helpers;
using HassClient.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Switch"/> domain.
    /// </summary>
    /// <remarks>
    /// Developers documentation: <see href="https://developers.home-assistant.io/docs/core/entity/switch"/>.
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/switch"/>.
    /// </remarks>
    public class SwitchEntity : Entity, ISwitchableEntity
    {
        private static KnownEnumCache<SwitchDeviceClass> knownDeviceClassCache = new KnownEnumCache<SwitchDeviceClass>(SwitchDeviceClass.None);

        /// <inheritdoc/>
        public bool IsOn => this.State.KnownState == KnownStates.On;

        /// <summary>
        /// Gets a value indicating whether the switch has an assumed state.
        /// <para>
        /// For switches with an assumed state two buttons are shown (turn off, turn on) instead of a switch.
        /// </para>
        /// </summary>
        public bool HasAssumedState => this.State.GetAttributeValue<bool>("assumed_state");

        /// <summary>
        /// Gets the class of the device.
        /// </summary>
        public SwitchDeviceClass DeviceClass => this.State.GetAttributeValue("device_class", knownDeviceClassCache);

        /// <summary>
        /// Gets the current power usage in watts (W).
        /// </summary>
        public double? CurrentPower => this.State.GetAttributeValue<double?>("current_power_w");

        /// <summary>
        /// Gets the today total energy usage in kilowatt hour (kWh).
        /// </summary>
        public double? TodayEnergy => this.State.GetAttributeValue<double?>("today_energy_kwh");

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal SwitchEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        /// <inheritdoc/>
        public Task<bool> TurnOnAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOn, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOffAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOff, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ToggleAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Toggle, cancellationToken);
        }
    }
}
