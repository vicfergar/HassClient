using HassClient.Entities.Collections;
using HassClient.Entities.Decorators;
using HassClient.Helpers;
using HassClient.Models;
using HassClient.WS;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities
{
    /// <summary>
    /// Represents a Home Assistant server instance. It provides a high lever abstraction to the Home Assistant API.
    /// </summary>
    /// <remarks>
    /// Once created this instance will be subscribed to Home Assistant event bus in order to maintain the entities and
    /// configuration in sync.</remarks>
    public class HassInstance : IDisposable
    {
        private const string TAG = "[" + nameof(HassInstance) + "]";

        private readonly CancellationTokenSource runningCTS;

        private readonly HassCollection<Entity> entitiesCollection = new HassCollection<Entity>();

        private StorageCollection<Area> areasCollection;

        private StorageCollection<Device> devicesCollection;

        private StorageCollection<User> usersCollection;

        /// <summary>
        /// <see cref="HassWSApi"/> instance used by this object.
        /// </summary>
        public HassWSApi HassWSApi { get; private set; }

        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        public ConnectionStates ConnectionState => this.HassWSApi.ConnectionState;

        /// <summary>
        /// Gets the configuration of the Home Assistant instance.
        /// </summary>
        public Configuration Configuration { get; private set; }

        /// <summary>
        /// Gets the existing entities in the Home Assistant instance.
        /// </summary>
        public IReadOnlyObservableCollection<Entity> Entities => this.entitiesCollection.Values;

        /// <summary>
        /// Gets the existing areas in the Home Assistant instance.
        /// </summary>
        public IReadOnlyObservableCollection<Area> Areas => this.areasCollection.Values;

        /// <summary>
        /// Gets the existing devices in the Home Assistant instance.
        /// </summary>
        public IReadOnlyObservableCollection<Device> Devices => this.devicesCollection.Values;

        /// <summary>
        /// Gets the existing users in the Home Assistant instance.
        /// </summary>
        public IReadOnlyObservableCollection<User> Users => this.usersCollection.Values;

        private HassInstance()
        {
            this.HassWSApi = new HassWSApi();
            this.runningCTS = new CancellationTokenSource();
        }

        /// <summary>
        /// Creates a <see cref="HassInstance"/> with the given <paramref name="connectionParameters"/>.
        /// </summary>
        /// <param name="connectionParameters">Connection parameters used to establish connection with the instance server.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is an instance of <see cref="HassInstance"/>.
        /// </returns>
        public static async Task<HassInstance> CreateAsync(ConnectionParameters connectionParameters)
        {
            var instace = new HassInstance();
            await instace.InitializeAsync(connectionParameters);
            return instace;
        }

        private async Task InitializeAsync(ConnectionParameters connectionParameters)
        {
            var cancelationToken = this.runningCTS.Token;

            this.HassWSApi.ConnectionStateChanged += this.HassWSApi_ConnectionStateChanged;
            await this.HassWSApi.ConnectAsync(connectionParameters, -1, cancelationToken);

            try
            {
                this.Configuration = new Configuration();
                await this.Configuration.UpdateAsync(this.HassWSApi, cancelationToken);
                var states = await this.HassWSApi.GetStatesAsync(cancelationToken);
                var sources = await this.HassWSApi.GetEntitySourcesAsync(cancelationToken);
                var entityRegistryEntries = await this.HassWSApi.GetEntitiesAsync(cancelationToken);
                var integrationManifests = await this.HassWSApi.GetIntegrationManifestsAsync(cancelationToken);
                var serviceDomains = await this.HassWSApi.GetServicesAsync(cancelationToken);

                var areas = await this.HassWSApi.GetAreasAsync(cancelationToken);
                var devices = await this.HassWSApi.GetDevicesAsync(cancelationToken);
                var users = await this.HassWSApi.GetUsersAsync(cancelationToken);
                this.areasCollection = new StorageCollection<Area>(areas);
                this.devicesCollection = new StorageCollection<Device>(devices);
                this.usersCollection = new StorageCollection<User>(users);

                var inputBooleans = await this.HassWSApi.GetStorageEntityRegistryEntriesAsync<InputBoolean>(cancelationToken);
                var persons = await this.HassWSApi.GetStorageEntityRegistryEntriesAsync<Person>(cancelationToken);
                var zones = await this.HassWSApi.GetStorageEntityRegistryEntriesAsync<Zone>(cancelationToken);
                var specificRegistryEntries = inputBooleans.Cast<StorageEntityRegistryEntryBase>()
                                                           .Concat(zones)
                                                           .Concat(persons);

                foreach (var state in states)
                {
                    var entityId = state.EntityId;
                    var domain = entityId.GetDomain();

                    var source = sources.FirstOrDefault(x => x.EntityId == entityId);
                    var serviceDomain = serviceDomains.FirstOrDefault(x => x.Domain == domain);
                    var registryEntry = entityRegistryEntries.FirstOrDefault(x => x.EntityId == entityId);
                    var specificRegistryEntry = specificRegistryEntries.FirstOrDefault(x => x.EntityId == entityId);

                    var def = new EntityDefinition(domain, state, source, serviceDomain, registryEntry, specificRegistryEntry);
                    var entity = this.CreateEntity(def);

                    this.entitiesCollection.Add(entityId, entity);
                }
            }
            catch (OperationCanceledException)
            {
                // TODO: Handle cancellation
            }
        }

        private void HassWSApi_ConnectionStateChanged(object sender, ConnectionStates state)
        {
            if (state == ConnectionStates.Disconnected)
            {
                // TODO: Invalidate instance state
                this.Configuration.MarkAsDirty();
                this.areasCollection.MarkAllEntriesAsDirty();
                this.devicesCollection.MarkAllEntriesAsDirty();
                this.usersCollection.MarkAllEntriesAsDirty();
            }
        }

        private Entity CreateEntity(EntityDefinition entityDefinition)
        {
            switch (entityDefinition.Domain.AsKnownDomain())
            {
                case KnownDomains.Automation: return new AutomationEntity(this, entityDefinition);

                case KnownDomains.DeviceTracker: return new DeviceTrackerEntity(this, entityDefinition);

                case KnownDomains.Fan: return new FanEntity(this, entityDefinition);

                case KnownDomains.InputBoolean: return new InputBooleanEntity(this, entityDefinition);

                case KnownDomains.Light: return new LightEntity(this, entityDefinition);

                case KnownDomains.PersistentNotification: return new PersistentNotificationEntity(this, entityDefinition);

                case KnownDomains.Person: return new PersonEntity(this, entityDefinition);

                case KnownDomains.Switch: return new SwitchEntity(this, entityDefinition);

                case KnownDomains.Zone: return new ZoneEntity(this, entityDefinition);

                default: return new Entity(this, entityDefinition);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // TODO: Clear all collections and unsubscribe from events.
            throw new NotImplementedException();
        }
    }
}
