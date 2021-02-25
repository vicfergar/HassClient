namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents known device tracker source types. Useful to reduce use of strings.
    /// </summary>
    public enum KnownDeviceTrackedSources
    {
        /// <summary>
        /// Used to represent a device tracker source type not defined within this enum.
        /// </summary>
        Unknown,

        /// <summary>
        /// Tracker source is a GPS sensor.
        /// </summary>
        GPS,

        /// <summary>
        /// Tracker source is a router that indicates the presence of the device.
        /// </summary>
        Router,

        /// <summary>
        /// Tracker source is a Bluetooth beacon.
        /// </summary>
        Bluetooth,

        /// <summary>
        /// Tracker source is a Bluetooth Low Energy beacon.
        /// </summary>
        BluetoothLE,
    }
}
