using HassClient.Helpers;
using HassClient.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an entity from the <see cref="KnownDomains.DeviceTracker"/> domain.
    /// </summary>
    /// <remarks>
    /// Developers documentation: <see href="https://developers.home-assistant.io/docs/core/entity/device-tracker"/>.
    /// Users documentation: <see href="https://www.home-assistant.io/integrations/device_tracker"/>.
    /// </remarks>
    public class DeviceTrackerEntity : Entity
    {
        private static KnownEnumCache<KnownDeviceTrackedSources> knownSourceTypesCache = new KnownEnumCache<KnownDeviceTrackedSources>();

        /// <summary>
        /// Gets the latitude of last position reported by the device tracker.
        /// </summary>
        public float Latitude => this.State.GetAttributeValue<float>("latitude");

        /// <summary>
        /// Gets the longitude of last position reported by the device tracker.
        /// </summary>
        public float Longitude => this.State.GetAttributeValue<float>("longitude");

        /// <summary>
        /// Gets the accuracy of the device tracker position in meters.
        /// </summary>
        public float GPSAccuracy => this.State.GetAttributeValue<float>("gps_accuracy");

        /// <summary>
        /// Gets the battery level of the device tracker.
        /// </summary>
        public float Battery => this.State.GetAttributeValue<float>("battery");

        /// <summary>
        /// Gets the device tracker source type as <see cref="string"/>.
        /// </summary>
        public string SourceTypeName => this.State.GetAttributeValue<string>("source_type");

        /// <summary>
        /// Gets a value indicating whether the device tracker is at home zone.
        /// </summary>
        public bool IsAtHome => this.State.KnownState == KnownStates.Home;

        /// <summary>
        /// Gets the device tracker source type as <see cref="KnownDeviceTrackedSources"/>.
        /// </summary>
        public KnownDeviceTrackedSources SourceType => knownSourceTypesCache.AsEnum(this.SourceTypeName);

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceTrackerEntity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal DeviceTrackerEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
        }

        /// <summary>
        /// Updates the state of a device tracker.
        /// </summary>
        /// <param name="parameters">Device tracker parameters.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// action was successfully done.
        /// </returns>
        public Task<bool> SeeAsync(DeviceTrackerParams parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            parameters.CheckValues();
            parameters.deviceId = this.EntityId.SplitEntityId()[1];
            return this.hassInstance.HassWSApi.CallServiceAsync(this.KnownDomain, KnownServices.See, parameters, cancellationToken);
        }
    }
}
