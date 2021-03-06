﻿using HassClient.Helpers;
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
    public class HassWSApi
    {
        private HassClientWebSocket hassClientWebSocket = new HassClientWebSocket();

        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        public ConnectionStates ConnectionState => this.hassClientWebSocket.ConnectionState;

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        public event EventHandler<ConnectionStates> ConnectionStateChanged
        {
            add => this.hassClientWebSocket.ConnectionStateChanged += value;
            remove => this.hassClientWebSocket.ConnectionStateChanged -= value;
        }

        /// <summary>
        /// Gets the <see cref="StateChagedEventListener"/> instance of this client instance.
        /// </summary>
        public StateChagedEventListener StateChagedEventListener { get; private set; }

        private async Task ConnectAsync(Uri uri, string accessToken)
        {
            await this.hassClientWebSocket.ConnectAsync(uri, accessToken);

            this.StateChagedEventListener = new StateChagedEventListener();
            this.StateChagedEventListener.Initialize(this.hassClientWebSocket);
        }

        /// <summary>
        /// Connects to a Home Assistant instance using the specified base URL and Access Token.
        /// </summary>
        /// <param name="instanceBaseUrl">
        /// An URL representing the Home Assistant instance base address. (e.g. "http://localhost:8123").
        /// </param>
        /// <param name="accessToken">
        /// The access token to be used during authentication phase.
        /// <para>
        /// You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser,
        /// and going to your profile http://IP_ADDRESS:8123/profile.
        /// </para>
        /// </param>
        /// <returns>A task representing the connection work.</returns>
        public Task ConnectAsClientAsync(string instanceBaseUrl, string accessToken)
        {
            if (string.IsNullOrEmpty(instanceBaseUrl))
            {
                throw new ArgumentNullException(nameof(instanceBaseUrl), $"{nameof(instanceBaseUrl)} is null or empty");
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            var uriBuilder = new UriBuilder(instanceBaseUrl);
            if (uriBuilder.Scheme == Uri.UriSchemeHttp)
            {
                uriBuilder.Scheme = "ws";
            }
            else if (uriBuilder.Scheme == Uri.UriSchemeHttps)
            {
                uriBuilder.Scheme = "wss";
            }
            else if (uriBuilder.Scheme != "ws" &&
                     uriBuilder.Scheme != "wss")
            {
                throw new ArgumentException("Invalid URI Scheme", nameof(instanceBaseUrl));
            }

            uriBuilder.Path = "/api/websocket";

            return this.ConnectAsync(uriBuilder.Uri, accessToken);
        }

        /// <summary>
        /// Connects to a Home Assistant instance using the add-ons internal proxy.
        /// </summary>
        /// <remarks>
        /// More information at <see href="https://developers.home-assistant.io/docs/add-ons/communication#home-assistant-core"/>.
        /// </remarks>
        /// <returns>A task representing the connection work.</returns>
        public Task ConnectFromAddonAsync()
        {
            const string tokenEnvVar = "SUPERVISOR_TOKEN";
            var supervisorToken = Environment.GetEnvironmentVariable(tokenEnvVar);

            if (string.IsNullOrEmpty(supervisorToken))
            {
                throw new InvalidOperationException($"Error initializing API as Supervisor: Environment variable '{tokenEnvVar}' not found.");
            }

            return this.ConnectAsync(new Uri("ws://supervisor/core/websocket"), supervisorToken);
        }

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
        public Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.AddEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

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
        public Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, KnownEventTypes eventType = KnownEventTypes.Any, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.RemoveEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

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
        public Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.AddEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

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
        public Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.RemoveEventHandlerSubscriptionAsync(value, eventType, cancellationToken);
        }

        /// <summary>
        /// Gets a dump of the configuration in use by the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is the <see cref="Configuration"/> object.
        /// </returns>
        public Task<Configuration> GetConfigurationAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetConfigMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<Configuration>(commandMessage, cancellationToken);
        }

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
        public async Task<IEnumerable<PanelInfo>> GetPanelsAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetPanelsMessage();
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, PanelInfo>>(commandMessage, cancellationToken);
            return dict?.Values;
        }

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
        public Task<IEnumerable<StateModel>> GetStatesAsync(CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<StateModel>>(new GetStatesMessage(), cancellationToken);
        }

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
        public async Task<Context> CallServiceAsync(string domain, string service, object data = null, CancellationToken cancellationToken = default)
        {
            var commandMessage = new CallServiceMessage(domain, service, data);
            var state = await this.hassClientWebSocket.SendCommandWithResultAsync<StateModel>(commandMessage, cancellationToken);
            return state?.Context;
        }

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
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// service invocation was successfully done.
        /// </returns>
        public async Task<bool> CallServiceAsync(KnownDomains domain, KnownServices service, object data = null, CancellationToken cancellationToken = default)
        {
            var context = await this.CallServiceAsync(domain.ToDomainString(), service.ToServiceString(), data, cancellationToken);
            return context != null;
        }

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
        public Task<bool> CallServiceForEntitiesAsync(string domain, string service, params string[] entityIds)
        {
            return this.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

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
        public async Task<bool> CallServiceForEntitiesAsync(string domain, string service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            var context = await this.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
            return context != null;
        }

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
        public Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, params string[] entityIds)
        {
            return this.CallServiceForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

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
        public Task<bool> CallServiceForEntitiesAsync(KnownDomains domain, KnownServices service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            return this.CallServiceAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
        }

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
        public async Task<string> RenderTemplateAsync(string template, CancellationToken cancellationToken = default)
        {
            var commandMessage = new RenderTemplateMessage() { Template = template };
            if (!await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken))
            {
                return default;
            }

            return await commandMessage.WaitResponseTask;
        }

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
        public Task<IntegrationManifest> GetIntegrationManifestAsync(string integrationName, CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetManifestMessage { Integration = integrationName };
            return this.hassClientWebSocket.SendCommandWithResultAsync<IntegrationManifest>(commandMessage, cancellationToken);
        }

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
        public Task<IEnumerable<IntegrationManifest>> GetIntegrationManifestsAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new ListManifestsMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<IntegrationManifest>>(commandMessage, cancellationToken);
        }

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
        public async Task<EntitySource> GetEntitySourceAsync(string entityId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentNullException(nameof(entityId));
            }

            var result = await this.GetEntitySourcesAsync(cancellationToken, entityId);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
        /// </summary>
        /// <param name="entityIds">The entities ids.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="EntitySource"/> of the specified entities.
        /// </returns>
        public Task<IEnumerable<EntitySource>> GetEntitySourcesAsync(params string[] entityIds)
        {
            return this.GetEntitySourcesAsync(CancellationToken.None, entityIds);
        }

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
        public Task<IEnumerable<EntityRegistryEntry>> GetEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<EntityRegistryEntry>>(commandMessage, cancellationToken);
        }

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
        public Task<EntityRegistryEntry> GetEntityAsync(string entityId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                throw new ArgumentException($"'{nameof(entityId)}' cannot be null or empty", nameof(entityId));
            }

            var commandMessage = EntityRegistryMessagesFactory.Instance.CreateGetMessage(entityId);
            return this.hassClientWebSocket.SendCommandWithResultAsync<EntityRegistryEntry>(commandMessage, cancellationToken);
        }

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
        public Task<IEnumerable<Area>> GetAreasAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = AreaRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Area>>(commandMessage, cancellationToken);
        }

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
        public Task<IEnumerable<Device>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = DeviceRegistryMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<Device>>(commandMessage, cancellationToken);
        }

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

        /// <summary>
        /// Gets a collection with every registered <see cref="InputBoolean"/> entity in the Home Assistant instance.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection with
        /// every registered <see cref="InputBoolean"/> entity in the Home Assistant instance.
        /// </returns>
        public Task<IEnumerable<InputBoolean>> GetInputBooleansAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = InputBooleanMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<InputBoolean>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="InputBoolean"/> entity.
        /// </summary>
        /// <param name="value">The new <see cref="InputBoolean"/> entity.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// create operation was successfully done.
        /// </returns>
        public async Task<bool> CreateInputBooleanAsync(InputBoolean value, CancellationToken cancellationToken = default)
        {
            var commandMessage = InputBooleanMessagesFactory.Instance.CreateCreateMessage(value);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(value);
            }

            return result.Success;
        }

        /// <summary>
        /// Updates an existing <see cref="InputBoolean"/> entity.
        /// </summary>
        /// <param name="inputBoolean">The <see cref="InputBoolean"/> entity with the new values.</param>
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
        public async Task<bool> UpdateInputBooleanAsync(InputBoolean inputBoolean, bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            var commandMessage = InputBooleanMessagesFactory.Instance.CreateUpdateMessage(inputBoolean, forceUpdate);
            var result = await this.hassClientWebSocket.SendCommandWithResultAsync(commandMessage, cancellationToken);
            if (result.Success)
            {
                result.PopulateResult(inputBoolean);
            }

            return result.Success;
        }

        /// <summary>
        /// Deletes an existing <see cref="InputBoolean"/> entity.
        /// </summary>
        /// <param name="inputBoolean">The <see cref="InputBoolean"/> entity to delete.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// delete operation was successfully done.
        /// </returns>
        public async Task<bool> DeleteInputBooleanAsync(InputBoolean inputBoolean, CancellationToken cancellationToken = default)
        {
            var commandMessage = InputBooleanMessagesFactory.Instance.CreateDeleteMessage(inputBoolean);
            var success = await this.hassClientWebSocket.SendCommandWithSuccessAsync(commandMessage, cancellationToken);
            if (success)
            {
                inputBoolean.Untrack();
            }

            return success;
        }

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
        public Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = UserMessagesFactory.Instance.CreateListMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<User>>(commandMessage, cancellationToken);
        }

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
        public Task<SearchRelatedResponse> SearchRelated(ItemTypes itemType, string itemId, CancellationToken cancellationToken = default)
        {
            var commandMessage = new SearchRelatedMessage(itemType, itemId);
            return this.hassClientWebSocket.SendCommandWithResultAsync<SearchRelatedResponse>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the <see cref="HassWSApi"/>.
        /// </summary>
        /// <param name="rawCommandMessage">The raw command message to send.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="RawCommandResult"/>
        /// with the response from the server.
        /// </returns>
        public async Task<RawCommandResult> SendRawCommandWithResultAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default)
        {
            var resultMessage = await this.hassClientWebSocket.SendCommandWithResultAsync(rawCommandMessage, cancellationToken);
            return RawCommandResult.FromResultMessage(resultMessage);
        }

        /// <summary>
        /// Sends a customized command to the Home Assistant instance. This is useful when a command is not defined by the <see cref="HassWSApi"/>.
        /// </summary>
        /// <param name="rawCommandMessage">The raw command message to send.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a <see cref="bool"/> indicating if
        /// the operation was successfully done.
        /// </returns>
        public Task<bool> SendRawCommandWithSuccessAsync(BaseOutgoingMessage rawCommandMessage, CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.SendCommandWithSuccessAsync(rawCommandMessage, cancellationToken);
        }
    }
}
