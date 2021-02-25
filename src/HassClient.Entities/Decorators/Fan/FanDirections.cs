using System;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents fan directions.
    /// </summary>
    [Flags]
    public enum FanDirections
    {
        /// <summary>
        /// The fan does not support <see cref="FanFeatures.SupportDirection"/> feature.
        /// </summary>
        None,

        /// <summary>
        /// The fan will rotate in forward direction.
        /// </summary>
        Forward,

        /// <summary>
        /// The fan will rotate in reverse direction.
        /// </summary>
        Reverse,
    }
}
