namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents known lights color modes. Useful to reduce use of strings.
    /// </summary>
    public enum LightColorModes
    {
        /// <summary>
        /// Used to represent a light color not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// The light can be turned on or off. This mode must be the only
        /// supported mode if supported by the light.
        /// </summary>
        OnOff,

        /// <summary>
        /// The light can be dimmed. This mode must be the only supported mode if
        /// supported by the light.
        /// </summary>
        Brightness,

        /// <summary>
        /// The light can be dimmed and its color temperature is present in the state.
        /// </summary>
        ColorTemp,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light's color can be set using the hs_color parameter and read through the
        /// <see cref="LightEntity.HSColor"/> property.
        /// </summary>
        HS,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light's color can be set using the rgb_color parameter and read through the
        /// <see cref="LightEntity.RGBColor"/> property.
        /// </summary>
        RGB,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light's color can be set using the rgb_color parameter and read through the
        /// <see cref="LightEntity.RGBWColor"/> property.
        /// </summary>
        RGBW,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light's color can be set using the rgb_color parameter and read through the
        /// <see cref="LightEntity.RGBWWColor"/> property.
        /// </summary>
        RGBWW,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light can be set to white mode by using the <see cref="LightParams.WhiteLevel"/>
        /// parameter with the desired brightness as value. Note that there's no white property.
        /// If both <see cref="LightParams.Brightness"/> and <see cref="LightParams.WhiteLevel"/>
        /// are present in a service call, the <see cref="LightParams.WhiteLevel"/> parameter will
        /// be updated with the value of brightness. If this mode is supported, the light must also
        /// support at least one of <see cref="HS"/>, <see cref="RGB"/>,
        /// <see cref="RGBW"/>, <see cref="RGBWW"/> or <see cref="XY"/>.
        /// </summary>
        White,

        /// <summary>
        /// The light can be dimmed and its color can be adjusted. The light's brightness
        /// can be set using the <see cref="LightParams.Brightness"/> parameter and read
        /// through the <see cref="LightEntity.Brightness"/> property.
        /// The light's color can be set using the xy_color parameter and read through the
        /// <see cref="LightEntity.XYColor"/> property.
        /// </summary>
        XY,
    }
}
