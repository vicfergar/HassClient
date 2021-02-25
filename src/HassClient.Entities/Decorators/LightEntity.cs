using HassClient.Helpers;
using HassClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.Light"/> domain.
    /// </summary>
    /// <remarks>
    /// Developers documentation: <see href="https://developers.home-assistant.io/docs/core/entity/light"/>.
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/light"/>.
    /// </remarks>
    public class LightEntity : Entity, ISwitchableEntity
    {
        internal static KnownEnumCache<KnownLightEffects> knownLightEffectCache = new KnownEnumCache<KnownLightEffects>(KnownLightEffects.None);

        internal static KnownEnumCache<LightColorModes> knownLightColorModesCache = new KnownEnumCache<LightColorModes>(LightColorModes.Unknown);

        /// <inheritdoc/>
        public bool IsOn => this.State.KnownState == KnownStates.On;

        /// <summary>
        /// Gets a flag value indicating the supported features of the light.
        /// </summary>
        public LightFeatures SupportedFeatures => (LightFeatures)this.State.GetAttributeValue<int>("supported_features");

        /// <summary>
        /// Gets a value indicating the supported light color mode.
        /// </summary>
        public LightColorModes ColorMode => (LightColorModes)knownLightColorModesCache.AsEnum(this.State.GetAttributeValue<string>("color_mode"));

        /// <summary>
        /// Gets a value indicating the supported light color mode.
        /// </summary>
        public IEnumerable<LightColorModes> SupportedColorModes => this.State.GetAttributeValue<string[]>("supported_color_modes").Select(x => knownLightColorModesCache.AsEnum(x));

        /// <summary>
        /// Gets the coldest <see cref="MiredsTemperatureColor"/> that this light supports.
        /// </summary>
        public int? MinMireds => this.State.GetAttributeValue<int?>("min_mireds");

        /// <summary>
        /// Gets the warmest <see cref="MiredsTemperatureColor"/> that this light supports.
        /// </summary>
        public int? MaxMireds => this.State.GetAttributeValue<int?>("max_mireds");

        /// <summary>
        /// Gets the color of the light represented as <see cref="RGBColor"/>.
        /// </summary>
        public RGBColor RGBColor => this.State.GetAttributeValue<RGBColor>("rgb_color");

        /// <summary>
        /// Gets the color of the light represented as <see cref="RGBWColor"/>.
        /// </summary>
        public RGBWColor RGBWColor => this.State.GetAttributeValue<RGBWColor>("rgbw_color");

        /// <summary>
        /// Gets the color of the light represented as <see cref="RGBWWColor"/>.
        /// </summary>
        public RGBWWColor RGBWWColor => this.State.GetAttributeValue<RGBWWColor>("rgbwW_color");

        /// <summary>
        /// Gets the color of the light represented as <see cref="XYColor"/>.
        /// </summary>
        public XYColor XYColor => this.State.GetAttributeValue<XYColor>("xy_color");

        /// <summary>
        /// Gets the color of the light represented as <see cref="HSColor"/>.
        /// </summary>
        public HSColor HSColor => this.State.GetAttributeValue<HSColor>("hs_color");

        /// <summary>
        /// Gets the color of the light represented as <see cref="MiredsTemperatureColor"/>.
        /// </summary>
        public MiredsTemperatureColor MiredsTemperatureColor =>
            this.State.GetAttributeValue<MiredsTemperatureColor>("color_temp");

        /// <summary>
        /// Gets the brightness of this light between 0 and 1.
        /// </summary>
        public double? Brightness => (double)this.BrightnessValue / 255;

        /// <summary>
        /// Gets the brightness of this light between 0 and 255.
        /// </summary>
        public int? BrightnessValue => this.State.GetAttributeValue<int?>("brightness");

        /// <summary>
        /// Gets the current effect as <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="EffectNameList"/>.
        /// </remarks>
        public string ActiveEffectName => this.State.GetAttributeValue<string>("effect");

        /// <summary>
        /// Gets the possible <see cref="ActiveEffectName"/> values.
        /// </summary>
        public IEnumerable<string> EffectNameList => this.State.GetAttributeValues<string>("effect_list");

        /// <summary>
        /// Gets the current effect as <see cref="KnownLightEffects"/>.
        /// </summary>
        /// <remarks>
        /// Possible values of this property are given by <see cref="EffectNameList"/>.
        /// </remarks>
        public KnownLightEffects ActiveEffect => knownLightEffectCache.AsEnum(this.ActiveEffectName);

        /// <summary>
        /// Gets the possible <see cref="ActiveEffect"/> values.
        /// </summary>
        public IEnumerable<KnownLightEffects> EffectList => this.EffectNameList
                                                                .Select(x => knownLightEffectCache.AsEnum(x))
                                                                .Where(x => x != KnownLightEffects.Unknown);

        /// <summary>
        /// Initializes a new instance of the <see cref="LightEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal LightEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        /// <inheritdoc/>
        public Task<bool> ToggleAsync(CancellationToken cancellationToken = default)
        {
            return this.ToggleAsync(null, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOffAsync(CancellationToken cancellationToken = default)
        {
            return this.TurnOffAsync(null, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> TurnOnAsync(CancellationToken cancellationToken = default)
        {
            return this.TurnOnAsync(null, cancellationToken);
        }

        /// <summary>
        /// Turns on the light.
        /// </summary>
        /// <param name="parameters">Optional parameters.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> TurnOnAsync(LightParams parameters, CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOn, parameters, cancellationToken);
        }

        /// <summary>
        /// Turns off the light.
        /// </summary>
        /// <param name="parameters">Optional parameters.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> TurnOffAsync(LightOffParams parameters, CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.TurnOff, parameters, cancellationToken);
        }

        /// <summary>
        /// Toggles the light.
        /// </summary>
        /// <param name="parameters">Optional parameters.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> ToggleAsync(LightParams parameters, CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Toggle, parameters, cancellationToken);
        }
    }
}
