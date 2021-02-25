using HassClient.Helpers;
using HassClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Fan"/> domain.
    /// </summary>
    /// <remarks>
    /// Developers documentation: <see href="https://developers.home-assistant.io/docs/core/entity/fan"/>.
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/fan"/>.
    /// </remarks>
    public class FanEntity : Entity, ISwitchableEntity
    {
        internal static KnownEnumCache<FanDirections> knownFanDirectionsCache = new KnownEnumCache<FanDirections>(FanDirections.None);

        internal static KnownEnumCache<KnownFanSpeeds> knownFanSpeedCache = new KnownEnumCache<KnownFanSpeeds>(KnownFanSpeeds.None);

        internal static KnownEnumCache<KnownFanPresetModes> knownFanPresetsCache = new KnownEnumCache<KnownFanPresetModes>(KnownFanPresetModes.None);

        /// <inheritdoc/>
        public bool IsOn => this.State.KnownState == KnownStates.On;

        /// <summary>
        /// Gets a flag value indicating the supported features of the fan.
        /// </summary>
        public FanFeatures SupportedFeatures => (FanFeatures)this.State.GetAttributeValue<int>("supported_features");

        /// <summary>
        /// Gets a value indicating whether the fan is oscillating.
        /// </summary>
        public bool? IsOscillating => this.State.GetAttributeValue<bool?>("oscillating");

        /// <summary>
        /// Gets the current speed percentage. Must be a value between 0 (off) and 100.
        /// </summary>
        public float? Percentage => this.State.GetAttributeValue<float?>("percentage");

        /// <summary>
        /// Gets the minimum percentage step used supported by the fan when invoking
        /// <see cref="IncreaseSpeedAsync(float, CancellationToken)"/> or <see cref="DecreaseSpeedAsync(float, CancellationToken)"/>.
        /// </summary>
        public float? PercentageStep => this.State.GetAttributeValue<float?>("percentage_step");

        /// <summary>
        /// Gets the direction in which the fan will rotate.
        /// </summary>
        public FanDirections Direction => this.State.GetAttributeValue("direction", knownFanDirectionsCache);

        /// <summary>
        /// Gets the current speed as <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="SpeedNameList"/>.
        /// </remarks>
        public string SpeedName => this.State.GetAttributeValue<string>("speed");

        /// <summary>
        /// Gets the possible <see cref="SpeedName"/> values.
        /// </summary>
        public IEnumerable<string> SpeedNameList => this.State.GetAttributeValues<string>("speed_list");

        /// <summary>
        /// Gets the current speed as a <see cref="KnownFanSpeeds"/>.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="SpeedList"/>.
        /// </remarks>
        public KnownFanSpeeds Speed => knownFanSpeedCache.AsEnum(this.SpeedName);

        /// <summary>
        /// Gets the possible <see cref="Speed"/> values.
        /// </summary>
        public IEnumerable<KnownFanSpeeds> SpeedList => this.SpeedNameList
                                                            .Select(x => knownFanSpeedCache.AsEnum(x))
                                                            .Where(x => x != KnownFanSpeeds.Unknown);

        /// <summary>
        /// Gets the current preset mode as <see cref="string"/>. A fan may have preset modes that automatically control the percentage speed or other functionality.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="PresetModeNameList"/>. If no preset mode is set, the <see cref="PresetModeName"/> property must be set to <c>None</c>.
        /// </remarks>
        public string PresetModeName => this.State.GetAttributeValue<string>("preset_mode");

        /// <summary>
        /// Gets the possible <see cref="PresetModeName"/> values.
        /// </summary>
        public IEnumerable<string> PresetModeNameList => this.State.GetAttributeValues<string>("preset_modes");

        /// <summary>
        /// Gets the current preset mode as <see cref="KnownFanPresetModes"/>. A fan may have preset modes that automatically control the percentage speed or other functionality.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="PresetModeNameList"/>. If no preset mode is set, the <see cref="PresetModeName"/> property must be set to <see cref="KnownFanPresetModes.None"/>.
        /// </remarks>
        public KnownFanPresetModes PresetMode => knownFanPresetsCache.AsEnum(this.PresetModeName);

        /// <summary>
        /// Gets the possible <see cref="PresetMode"/> values.
        /// </summary>
        public IEnumerable<KnownFanPresetModes> PresetModeList => this.PresetModeNameList
                                                                      .Select(x => knownFanPresetsCache.AsEnum(x))
                                                                      .Where(x => x != KnownFanPresetModes.Unknown);

        /// <summary>
        /// Initializes a new instance of the <see cref="FanEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal FanEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        /// <inheritdoc/>
        public Task<bool> ToggleAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Toggle, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOffAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOff, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOnAsync(CancellationToken cancellationToken = default)
        {
            return this.TurnOnAsync(null, cancellationToken);
        }

        /// <summary>
        /// Turns on the fan.
        /// </summary>
        /// <param name="parameters">Optional parameters.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> TurnOnAsync(FanOnParams parameters, CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOn, parameters, cancellationToken);
        }

        /// <summary>
        /// Decreases the speed of the fan when supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        /// <param name="percentageStep">
        /// The speed as percentage. Should be between <c>0</c> and <c>100</c>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> DecreaseSpeedAsync(float percentageStep, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { SpeedPercentageStep = percentageStep };
            return this.CallServiceAsync(KnownServices.DecreaseSpeed, data, cancellationToken);
        }

        /// <summary>
        /// Increases the speed of the fan when supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        /// <param name="percentageStep">
        /// The speed as percentage. Should be between <c>0</c> and <c>100</c>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> IncreaseSpeedAsync(float percentageStep, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { SpeedPercentageStep = percentageStep };
            return this.CallServiceAsync(KnownServices.IncreaseSpeed, data, cancellationToken);
        }

        /// <summary>
        /// Sets the speed of the fan when supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        /// <param name="percentage">
        /// The speed as percentage. Should be between <c>0</c> and <c>100</c>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetSpeedPercentageAsync(float percentage, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { SpeedPercentage = percentage };
            return this.CallServiceAsync(KnownServices.SetPercentage, data, cancellationToken);
        }

        /// <summary>
        /// Sets the speed of the fan when supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        /// <param name="speed">
        /// The speed as <see cref="KnownFanSpeeds"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetSpeedAsync(KnownFanSpeeds speed, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { Speed = speed };
            return this.CallServiceAsync(KnownServices.SetSpeed, data, cancellationToken);
        }

        /// <summary>
        /// Sets the speed of the fan when supports <see cref="FanFeatures.SupportSetSpeed"/>.
        /// </summary>
        /// <param name="speed">
        /// The speed name.
        /// <para>It is recommended to use <see cref="SetSpeedAsync(KnownFanSpeeds, CancellationToken)"/>
        /// overload instead when possible to reduce use of strings.</para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetSpeedAsync(string speed, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { SpeedName = speed };
            return this.CallServiceAsync(KnownServices.SetSpeed, data, cancellationToken);
        }

        /// <summary>
        /// Sets the oscillating of the fan when supports <see cref="FanFeatures.SupportOscillate"/>.
        /// </summary>
        /// <param name="oscillating">A value indicating whether the fan should oscillate.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetOscillateAsync(bool oscillating, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { Oscillating = oscillating };
            return this.CallServiceAsync(KnownServices.Oscillate, data, cancellationToken);
        }

        /// <summary>
        /// Sets the direction of the fan when supports <see cref="FanFeatures.SupportDirection"/>.
        /// </summary>
        /// <param name="direction">The fan direction.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetDirectionAsync(FanDirections direction, CancellationToken cancellationToken = default)
        {
            if (direction == FanDirections.None)
            {
                return Task.FromResult(false);
            }

            var data = new FanParams() { Direction = direction };
            return this.CallServiceAsync(KnownServices.SetDirection, data, cancellationToken);
        }

        /// <summary>
        /// Sets a preset mode for the fan when supports <see cref="FanFeatures.SupportPresetMode"/>.
        /// </summary>
        /// <param name="presetMode">The fan preset mode to use.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetPresetModeAsync(KnownFanPresetModes presetMode, CancellationToken cancellationToken = default)
        {
            if (presetMode == KnownFanPresetModes.Unknown)
            {
                return Task.FromResult(false);
            }

            var data = new FanParams() { PresetMode = presetMode };
            return this.CallServiceAsync(KnownServices.SetPresetMode, data, cancellationToken);
        }

        /// <summary>
        /// Sets a preset mode for the fan when supports <see cref="FanFeatures.SupportPresetMode"/>.
        /// </summary>
        /// <param name="presetMode">
        /// The fan preset mode name to use.
        /// <para>It is recommended to use <see cref="SetPresetModeAsync(KnownFanPresetModes, CancellationToken)"/>
        /// overload instead when possible to reduce use of strings.</para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SetPresetModeAsync(string presetMode, CancellationToken cancellationToken = default)
        {
            var data = new FanParams() { PresetModeName = presetMode };
            return this.CallServiceAsync(KnownServices.SetPresetMode, data, cancellationToken);
        }
    }
}
