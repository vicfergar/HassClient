using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Decorator for entities that provides better access to state, attributes and services invocation.
    /// </summary>
    public class Entity : IDisposable
    {
        /// <summary>
        /// The <see cref="HassInstance"/> dependency.
        /// </summary>
        protected HassInstance hassInstance;

        private bool disposed;

        private EventHandler<StateChangedEvent> stateChanged;

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        public string EntityId => this.State.EntityId;

        /// <summary>
        /// Gets the domain of the entity as <see cref="string"/>.
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets the domain of the entity as <see cref="KnownDomains"/>.
        /// </summary>
        public KnownDomains KnownDomain { get; private set; }

        /// <summary>
        /// Gets the latest state of the entity.
        /// </summary>
        public StateModel State { get; private set; }

        /// <summary>
        /// Gets the <see cref="EntitySource"/> associated to this entity.
        /// </summary>
        public EntitySource Source { get; private set; }

        /// <summary>
        /// Gets the <see cref="Models.ServiceDomain"/> associated to this entity.
        /// </summary>
        public ServiceDomain ServiceDomain { get; private set; }

        /// <summary>
        /// Gets the <see cref="Models.EntityRegistryEntry"/> associated to this entity.
        /// </summary>
        public EntityRegistryEntry EntityRegistryEntry { get; private set; }

        /// <summary>
        /// Gets the <see cref="Models.Area"/> associated to this entity.
        /// </summary>
        public Area Area
        {
            get
            {
                var areaId = this.EntityRegistryEntry?.AreaId;
                return areaId != null ? this.hassInstance.Areas.FindById(areaId) : null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Models.Device"/> associated to this entity.
        /// </summary>
        public Device Device
        {
            get
            {
                var deviceId = this.EntityRegistryEntry?.DeviceId;
                return deviceId != null ? this.hassInstance.Devices.FindById(deviceId) : null;
            }
        }

        /// <summary>
        /// Gets the name of the entity as displayed in the UI.
        /// </summary>
        public string FriendlyName => this.State.GetAttributeValue<string>("friendly_name");

        /// <summary>
        /// Gets the icon of the entity displayed in the UI.
        /// </summary>
        public string Icon => this.State.GetAttributeValue<string>("icon");

        /// <summary>
        /// Occurs when the state of the entity changes.
        /// </summary>
        public event EventHandler<StateChangedEvent> StateChanged
        {
            add
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(nameof(HashCode));
                }

                this.stateChanged += value;
            }

            remove
            {
                this.stateChanged -= value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected internal Entity(HassInstance hassInstance, EntityDefinition entityDefinition)
        {
            this.Domain = entityDefinition.Domain;
            this.KnownDomain = this.Domain.AsKnownDomain();
            this.State = entityDefinition.State;
            this.Source = entityDefinition.Source;
            this.ServiceDomain = entityDefinition.ServiceDomain;
            this.EntityRegistryEntry = entityDefinition.EntityRegistryEntry;

            this.hassInstance = hassInstance;
            this.hassInstance.HassWSApi.StateChagedEventListener.SubscribeEntityStatusChanged(this.EntityId, this.OnStateChanged);
        }

        private void OnStateChanged(object sender, StateChangedEvent stateChanged)
        {
            this.State = stateChanged.NewState;
            this.stateChanged?.Invoke(this, stateChanged);
        }

        /// <summary>
        /// Calls a service from entity domain.
        /// </summary>
        /// <param name="service">The service to call.</param>
        /// <param name="data">The optional data to use in the service invocation.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        public async Task<bool> CallServiceAsync(string service, object data = null, CancellationToken cancellationToken = default)
        {
            if (this.ServiceDomain == null)
            {
                throw new InvalidOperationException($"The domain '{this.Domain}' as no services available");
            }

            if (!this.ServiceDomain.Services.ContainsKey(service))
            {
                throw new InvalidOperationException($"'{service}' is not a valid service for the '{this.Domain}' domain");
            }

            var dataJObject = data != null ? HassSerializer.CreateJObject(data) : new JObject();
            dataJObject.TryAdd(HassSerializer.GetDefaultSerializedPropertyName(nameof(this.EntityId)), this.EntityId);

            var context = await this.hassInstance.HassWSApi.CallServiceAsync(this.Domain, service, dataJObject, cancellationToken);
            return context != null;
        }

        /// <summary>
        /// Calls a service from entity domain.
        /// </summary>
        /// <param name="service">The service to call.</param>
        /// <param name="data">The optional data to use in the service invocation.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        public Task<bool> CallServiceAsync(KnownServices service, object data = null, CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(service.ToServiceString(), data, cancellationToken);
        }

        /// <summary>
        /// Performs a search related operation for this entity.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="SearchRelatedResult"/>
        /// with all found relations.
        /// </returns>
        public async Task<SearchRelatedResult> SearchRelatedAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.hassInstance.HassWSApi.SearchRelatedAsync(ItemTypes.Entity, this.EntityId, cancellationToken);
            return new SearchRelatedResult(response, this.hassInstance);
        }

        /// <inheritdoc/>
        public override string ToString() => string.Join(' ', this.FriendlyName, $"[{this.EntityId}]");

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.hassInstance.HassWSApi.StateChagedEventListener.UnsubscribeEntityStatusChanged(this.EntityId, this.OnStateChanged);
            }
        }
    }
}
