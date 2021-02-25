namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents the device’s class configuration option.
    /// </summary>
    public enum SwitchDeviceClass
    {
        /// <summary>
        /// (Default) Generic switch. This is the default and doesn’t need to be set.
        /// </summary>
        None,

        /// <summary>
        /// This switch, switches a power outlet.
        /// </summary>
        Outlet,

        /// <summary>
        /// A generic switch.
        /// </summary>
        Switch,
    }
}
