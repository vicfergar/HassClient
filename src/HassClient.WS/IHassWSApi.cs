using HassClient.Models;
using HassClient.WS.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Web Socket client to interact with a Home Assistant instance.
    /// </summary>
    public interface IHassWSApi
    {
        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        ConnectionStates ConnectionState { get; }

        /// <summary>
        /// Gets the <see cref="StateChagedEventListener"/> instance of this client instance.
        /// </summary>
        StateChangedEventListener StateChagedEventListener { get; }

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        event EventHandler<ConnectionStates> ConnectionStateChanged;

        /// <summary>
        /// Subscribes an <see cref="EventHandler{EventResultInfo}"/> to handle events received from the Home Assistance instance.
        /// </summary>
        /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be included.</param>
        /// <param name="eventType">The event type to listen to. By default, no filter will be applied.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// subscription was successfully done.
        /// </returns>
        Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes an <see cref="EventHandler{EventResultInfo}"/> to handle events received from the Home Assistance instance.
        /// </summary>
        /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be included.</param>
        /// <param name="eventType">The event type to listen to. By default, no filter will be applied.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// subscription was successfully done.
        /// </returns>
        Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a service within a specific domain.
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="data">The optional data to use in the service invocation.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="Context"/>
        /// associated with the result of the service invocation.
        /// </returns>
        Task<bool> CallServiceAsync(KnownDomains domain, KnownServices service, object data = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a service within a specific domain.
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="data">The optional data to use in the service invocation.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="Context"/>
        /// associated with the result of the service invocation.
        /// </returns>
        Task<Context> CallServiceAsync(string domain, string service, object data = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a service within a specific domain and entities.
        /// <para>
        /// This overload is useful when only entity_id is needed in service invocation.
        /// </para>
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, CancellationToken cancellationToken = default, params string[] entityIds);

        /// <summary>
        /// Calls a service within a specific domain and entities.
        /// <para>
        /// This overload is useful when only entity_id is needed in service invocation.
        /// </para>
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, params string[] entityIds);

        /// <summary>
        /// Calls a service within a specific domain and entities.
        /// <para>
        /// This overload is useful when only entity_id is needed in service invocation.
        /// </para>
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> CallServiceForEntitiesAsync(string domain, string service, CancellationToken cancellationToken = default, params string[] entityIds);

        /// <summary>
        /// Calls a service within a specific domain and entities.
        /// <para>
        /// This overload is useful when only entity_id is needed in service invocation.
        /// </para>
        /// </summary>
        /// <param name="domain">The service domain.</param>
        /// <param name="service">The service to call.</param>
        /// <param name="entityIds">The ids of the target entities affected by the service call.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> CallServiceForEntitiesAsync(string domain, string service, params string[] entityIds);

        /// <summary>
        /// Close the Home Assistant connection as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task CloseAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Connects to a Home Assistant instance using the specified connection parameters.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="retries">
        /// Number of retries if connection failed. Default: 0.
        /// <para>
        /// Retries will only be performed if Home Assistant instance cannot be reached and not if:
        /// authentication fails OR
        /// invalid response from server OR
        /// connection refused by server.
        /// </para>
        /// <para>
        /// If set to <c>-1</c>, this method will try indefinitely until connection succeed or
        /// cancellation is requested. Therefore, <paramref name="cancellationToken"/> must be set
        /// to a value different to <see cref="CancellationToken.None"/> in that case.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>A task representing the connection work.</returns>
        Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> with the new values.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        Task<bool> CreateAreaAsync(Area area, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The new storage entity registry entry.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        Task<bool> CreateStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase;

        /// <summary>
        /// Creates a new <see cref="User"/>.
        /// </summary>
        /// <param name="user">The new <see cref="User"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        Task<bool> DeleteAreaAsync(Area area, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing <see cref="EntityRegistryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="EntityRegistryEntry"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        Task<bool> DeleteEntityAsync(EntityRegistryEntry entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The storage entity registry entry to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        Task<bool> DeleteStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase;

        /// <summary>
        /// Deletes an existing <see cref="User"/>.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        Task<bool> DeleteUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires an event on the Home Assistant event bus.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="data">The event optional data.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> FireEventAsync(KnownEventTypes eventType, object data = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires an event on the Home Assistant event bus.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="data">The event optional data.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        Task<bool> FireEventAsync(string eventType, object data = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with every registered <see cref="Area"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Area"/> in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<Area>> GetAreasAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a dump of the configuration in use by the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="ConfigurationModel"/> object.
        /// </returns>
        Task<ConfigurationModel> GetConfigurationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with every registered <see cref="Device"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="Device"/> in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<Device>> GetDevicesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with the <see cref="EntityRegistryEntry"/> of every registered entity in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="EntityRegistryEntry"/> of every registered entity in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<EntityRegistryEntry>> GetEntitiesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="EntityRegistryEntry"/> of a specified entity.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="EntityRegistryEntry"/>.
        /// </returns>
        Task<EntityRegistryEntry> GetEntityAsync(string entityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="EntitySource"/> of a specified entity.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="EntitySource"/>.
        /// </returns>
        Task<EntitySource> GetEntitySourceAsync(string entityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <param name="entityIds">The entities ids.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="EntitySource"/> of the specified entities.
        /// </returns>
        Task<IEnumerable<EntitySource>> GetEntitySourcesAsync(CancellationToken cancellationToken, params string[] entityIds);

        /// <summary>
        /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
        /// </summary>
        /// <param name="entityIds">The entities ids.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="EntitySource"/> of the specified entities.
        /// </returns>
        Task<IEnumerable<EntitySource>> GetEntitySourcesAsync(params string[] entityIds);

        /// <summary>
        /// Gets the <see cref="IntegrationManifest"/> that contains basic information about the specified integration.
        /// </summary>
        /// <param name="integrationName">The integration name.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="IntegrationManifest"/>
        /// containing basic information about the specified integration.
        /// </returns>
        Task<IntegrationManifest> GetIntegrationManifestAsync(string integrationName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with the <see cref="IntegrationManifest"/> that contains basic information of every
        /// registered integration in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="IntegrationManifest"/> of every registered integration in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<IntegrationManifest>> GetIntegrationManifestsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="PanelInfo"/> of the panel located at the specified <paramref name="urlPath"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="urlPath">The URL path of the panel.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="PanelInfo"/> object.
        /// </returns>
        Task<PanelInfo> GetPanelAsync(string urlPath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with the <see cref="PanelInfo"/> for every registered panel in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="PanelInfo"/> of every registered panel in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<PanelInfo>> GetPanelsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="ServiceDomain"/> of every registered service in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="ServiceDomain"/> of every registered service in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<ServiceDomain>> GetServicesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with the state of every registered entity in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// the state of every registered entity in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<StateModel>> GetStatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection with every registered storage entity registry entry of the given type
        /// in the Home Assistant instance.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <typeparamref name="TStorageEntity"/> entity in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<TStorageEntity>> GetStorageEntityRegistryEntriesAsync<TStorageEntity>(CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase;

        /// <summary>
        /// Gets a collection with every registered <see cref="User"/> in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="User"/> in the Home Assistant instance.
        /// </returns>
        Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh the configuration in use by the Home Assistant instance.
        /// </summary>
        /// <param name="configuration">The configuration model to be refreshed.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// refresh operation was successfully done.
        /// </returns>
        Task<bool> RefreshConfigurationAsync(ConfigurationModel configuration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh a given <see cref="EntityRegistryEntry"/> with the values from the server.
        /// </summary>
        /// <param name="entityRegistryEntry">The entity registry entry to refresh.</param>
        /// <param name="newEntityId">If not <see langword="null"/>, it will be used as entity id.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// refresh operation was successfully done.
        /// </returns>
        Task<bool> RefreshEntityAsync(EntityRegistryEntry entityRegistryEntry, string newEntityId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an <see cref="EventHandler{EventResultInfo}"/> subscription.
        /// </summary>
        /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be removed.</param>
        /// <param name="eventType">The event type filter used in the subscription.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// subscription removal was successfully done.
        /// </returns>
        Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an <see cref="EventHandler{TEventArgs}"/> subscription.
        /// </summary>
        /// <param name="value">The <see cref="EventHandler{EventResultInfo}"/> to be removed.</param>
        /// <param name="eventType">The event type filter used in the subscription.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// subscription removal was successfully done.
        /// </returns>
        Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renders a string using the template feature of Home Assistant.
        /// <para>
        /// More information at <see href="https://www.home-assistant.io/docs/configuration/templating/"/>.
        /// </para>
        /// </summary>
        /// <param name="template">The template input <see cref="string"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the rendered <see cref="string"/>.
        /// </returns>
        Task<string> RenderTemplateAsync(string template, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a search related operation for the specified item id.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="itemId">The item unique id.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="SearchRelatedResponse"/>
        /// with all found relations.
        /// </returns>
        Task<SearchRelatedResponse> SearchRelatedAsync(ItemTypes itemType, string itemId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the <see cref="IHassWSApi"/>.
        /// </summary>
        /// <param name="rawCommandMessage">The raw command message to send.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="RawCommandResult"/>
        /// with the response from the server.
        /// </returns>
        Task<RawCommandResult> SendRawCommandWithResultAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the <see cref="IHassWSApi"/>.
        /// </summary>
        /// <param name="rawCommandMessage">The raw command message to send.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="bool"/> indicating if
        /// the operation was successfully done.
        /// </returns>
        Task<bool> SendRawCommandWithSuccessAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing <see cref="Area"/>.
        /// </summary>
        /// <param name="area">The <see cref="Area"/> with the new values.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        Task<bool> UpdateAreaAsync(Area area, bool forceUpdate = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing <see cref="Device"/>.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> with the new values.</param>
        /// <param name="disable">If not <see langword="null"/>, it will enable or disable the entity.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        Task<bool> UpdateDeviceAsync(Device device, bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing <see cref="EntityRegistryEntry"/> with the specified data.
        /// </summary>
        /// <param name="entity">The <see cref="EntityRegistryEntry"/> with the new values.</param>
        /// <param name="newEntityId">If not <see langword="null"/>, it will update the current entity id.</param>
        /// <param name="disable">If not <see langword="null"/>, it will enable or disable the entity.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        Task<bool> UpdateEntityAsync(EntityRegistryEntry entity, string newEntityId = null, bool? disable = null, bool forceUpdate = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing storage entity registry entry of the given type.
        /// </summary>
        /// <typeparam name="TStorageEntity">The storage entity registry entry type.</typeparam>
        /// <param name="storageEntity">The storage entity registry entry with the updated values.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        Task<bool> UpdateStorageEntityRegistryEntryAsync<TStorageEntity>(TStorageEntity storageEntity, bool forceUpdate = false, CancellationToken cancellationToken = default)
            where TStorageEntity : StorageEntityRegistryEntryBase;

        /// <summary>
        /// Updates an existing <see cref="User"/>.
        /// </summary>
        /// <param name="user">The <see cref="User"/> with the new values.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update operation should force the update of every modifiable property.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// update operation was successfully done.
        /// </returns>
        Task<bool> UpdateUserAsync(User user, bool forceUpdate = false, CancellationToken cancellationToken = default);
    }
}
