using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Web Socket client to interact with a Home Assistant instance.
    /// </summary>
    public class HassWSApi : IHassWSApi
    {
        private HassClientWebSocket hassClientWebSocket = new HassClientWebSocket();

        /// <inheritdoc />
        public ConnectionStates ConnectionState => this.hassClientWebSocket.ConnectionState;

        /// <inheritdoc />
        public event EventHandler<ConnectionStates> ConnectionStateChanged
        {
            add => this.hassClientWebSocket.ConnectionStateChanged += value;
            remove => this.hassClientWebSocket.ConnectionStateChanged -= value;
        }

        /// <inheritdoc />
        public StateChangedEventListener StateChagedEventListener { get; private set; }

        /// <inheritdoc />
        public async Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default)
        {
            await this.hassClientWebSocket.ConnectAsync(connectionParameters, retries, cancellationToken);

            this.StateChagedEventListener = new StateChangedEventListener(this.hassClientWebSocket);
        }

        /// <inheritdoc />
        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.CloseAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default)
        {
            return this.AddEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default)
        {
            return this.RemoveEventHandlerSubscriptionAsync(value, eventType.ToEventTypeString(), cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.AddEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.RemoveEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ConfigurationModel> GetConfigurationAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetConfigMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<ConfigurationModel>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> RefreshConfigurationAsync(ConfigurationModel configuration, CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetConfigMessage();
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (!result.Success)
            {
                return false;
            }

            result.PopulateResult(configuration);
            return true;
        }

        /// <inheritdoc />
        public async Task<PanelInfo> GetPanelAsync(string urlPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                throw new ArgumentException($"'{nameof(urlPath)}' cannot be null or empty", nameof(urlPath));
            }

            var commandMessage = new GetPanelsMessage();
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, PanelInfo>>(commandMessage, cancellationToken);
            if (dict != null &&
                dict.TryGetValue(urlPath, out var result))
            {
                return result;
            }

            return default;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PanelInfo>> GetPanelsAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetPanelsMessage();
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, PanelInfo>>(commandMessage, cancellationToken);
            return dict?.Values;
        }

        /// <inheritdoc />
        public Task<IEnumerable<StateModel>> GetStatesAsync(CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<StateModel>>(new GetStatesMessage(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ServiceDomain>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetServicesMessage();
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, JRaw>>(commandMessage, cancellationToken);
            return dict?.Select(x =>
                new ServiceDomain()
                {
                    Domain = x.Key,
                    Services = HassSerializer.DeserializeObject<Dictionary<string, Service>>(x.Value),
                });
        }

        /// <inheritdoc />
        public async Task<Context> CallServiceAsync(string domain, string service, object data = null, CancellationToken cancellationToken = default)
        {
            var commandMessage = new CallServiceMessage(domain, service, data);
            var state = await this.hassClientWebSocket.SendCommandWithResultAsync<StateModel>(commandMessage, cancellationToken);
            return state?.Context;
        }

        /// <inheritdoc />
        public async Task<bool> CallServiceAsync(KnownDomains domain, KnownServices service, object data = null, CancellationToken cancellationToken = default)
        {
            var context = await this.CallServiceAsync(domain.ToDomainString(), service.ToServiceString(), data, cancellationToken);
            return context != null;
        }

        /// <inheritdoc />
        public Task<bool> CallServiceForEntitiesAsync(string domain, string service, params string[] entityIds)
        {
            return this.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

        /// <inheritdoc />
        public async Task<bool> CallServiceForEntitiesAsync(string domain, string service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            var context = await this.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
            return context != null;
        }

        /// <inheritdoc />
        public Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, params string[] entityIds)
        {
            return this.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

        /// <inheritdoc />
        public Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            return this.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> FireEventAsync(string eventType, object data = null, CancellationToken cancellationToken = default)
        {
            var commandMessage = new FireEventMessage(eventType, data);
            return this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> FireEventAsync(KnownEventTypes eventType, object data = null, CancellationToken cancellationToken = default)
        {
            return this.FireEventAsync(eventType.ToEventTypeString(), data, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> RenderTemplateAsync(string template, CancellationToken cancellationToken = default)
        {
            var commandMessage = new RenderTemplateMessage() { Template = template };
            if (!await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken))
            {
                return default;
            }

            return await commandMessage.WaitResponseTask;
        }

        /// <inheritdoc />
        public Task<IntegrationManifest> GetIntegrationManifestAsync(string integrationName, CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetManifestMessage { Integration = integrationName };
            return this.hassClientWebSocket.SendCommandWithResultAsync<IntegrationManifest>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IntegrationManifest>> GetIntegrationManifestsAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new ListManifestsMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<IntegrationManifest>>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EntitySource> GetEntitySourceAsync(string entityId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentNullException(nameof(entityId));
            }

            var result = await this.GetEntitySourcesAsync(cancellationToken, entityId);
            return result.FirstOrDefault();
        }

        /// <inheritdoc />
        public Task<IEnumerable<EntitySource>> GetEntitySourcesAsync(params string[] entityIds)
        {
            return this.GetEntitySourcesAsync(CancellationToken.None, entityIds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EntitySource>> GetEntitySourcesAsync(CancellationToken cancellationToken, params string[] entityIds)
        {
            var commandMessage = new EntitySourceMessage { EntityIds = entityIds.Length > 0 ? entityIds : null };
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, EntitySource>>(commandMessage, cancellationToken);
            return dict?.Select(x =>
                {
                    var entitySource = x.Value;
                    entitySource.EntityId = x.Key;
                    return entitySource;
                });
        }

        /// <inheritdoc />
        public Task<IEnumerable<EntityRegistryEntry>> GetEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<EntityRegistryEntry>>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public Task<EntityRegistryEntry> GetEntityAsync(string entityId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException($"'{nameof(entityId)}' cannot be null or empty", nameof(entityId));
            }

            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
            return this.hassClientWebSocket.SendCommandWithResultAsync<EntityRegistryEntry>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> RefreshEntityAsync(EntityRegistryEntry entityRegistryEntry, string newEntityId = null, CancellationToken cancellationToken = default)
        {
            var entityId = newEntityId ?? entityRegistryEntry.EntityId;
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (!result.Success)
            {
                return false;
            }

            result.PopulateResult(entityRegistryEntry);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateEntityAsync(EntityRegistryEntry entity, string newEntityId = null, bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            if (newEntityId == entity.EntityId)
            {
                throw new ArgumentException($"{nameof(newEntityId)} cannot be the same as {nameof(entity.EntityId)}");
            }

            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateUpdateMessage(entity, newEntityId, disable, forceUpdate);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync<EntityEntryResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.EntityEntryRaw, entity);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteEntityAsync(EntityRegistryEntry entity, CancellationToken cancellationToken = default)
        {
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateDeleteMessage(entity);
            var success = await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                entity.Untrack();
            }

            return success;
        }

        /// <inheritdoc />
        public Task<IEnumerable<Area>> GetAreasAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Area>>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CreateAreaAsync(Area area, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.CreateCreateMessage(area);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(area);
            }

            return result.Success;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAreaAsync(Area area, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.CreateUpdateMessage(area, forceUpdate);

            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(area);
            }

            return result.Success;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAreaAsync(Area area, CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.CreateDeleteMessage(area);
            var success = await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                area.Untrack();
            }

            return success;
        }

        /// <inheritdoc />
        public Task<IEnumerable<Device>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = DeviceRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Device>>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateDeviceAsync(Device device, bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = DeviceRegistryMessagesFactory.Instance.CreateUpdateMessage(device, disable, forceUpdate);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(device);
            }

            return result.Success;
        }

        /// <inheritdoc />
        public Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<User>>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.CreateCreateMessage(user);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.UserRaw, user);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(User user, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.CreateUpdateMessage(user, forceUpdate);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync<UserResponse>(commandMessage, cancellationToken);
            if (result == null)
            {
                return false;
            }

            HassSerializer.PopulateObject(result.UserRaw, user);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.CreateDeleteMessage(user);
            var success = await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                user.Untrack();
            }

            return success;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TStorageEntity>> GetStorageEntityRegistryEntriesAsync<TStorageEntity>(CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateListMessage();
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                if (typeof(TStorageEntity) == typeof(Person))
                {
                    var response = result.DeserializeResult<PersonResponse>();
                    return response.Storage
                                   .Select(person =>
                                   {
                                       person.IsStorageEntry = true;
                                       return person;
                                   })
                                   .Concat(response.Config)
                                   .Cast<TStorageEntity>();
                }

                return result.DeserializeResult<IEnumerable<TStorageEntity>>();
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<bool> CreateStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateCreateMessage(storageEntity);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(storageEntity);
            }

            return result.Success;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, bool forceUpdate = false, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateUpdateMessage(storageEntity, forceUpdate);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(storageEntity);
            }

            return result.Success;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase
        {
            var commandMessage = StorageCollectionMessagesFactory<TStorageEntity>.Create().CreateDeleteMessage(storageEntity);
            var success = await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                storageEntity.Untrack();
            }

            return success;
        }

        /// <inheritdoc />
        public Task<SearchRelatedResponse> SearchRelatedAsync(ItemTypes itemType, string itemId, CancellationToken cancellationToken = default)
        {
            var commandMessage = new SearchRelatedMessage(itemType, itemId);
            return this.hassClientWebSocket.SendCommandWithResultAsync<SearchRelatedResponse>(commandMessage, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<RawCommandResult> SendRawCommandWithResultAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default)
        {
            var resultMessage = await this.hassClientWebSocket.SendCommandWithResultAsync(rawCommandMessage, cancellationToken);
            return RawCommandResult.FromResultMessage(resultMessage);
        }

        /// <inheritdoc />
        public Task<bool> SendRawCommandWithSuccessAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.SendCommandWithSuccessAsync(rawCommandMessage, cancellationToken);
        }
    }
}
