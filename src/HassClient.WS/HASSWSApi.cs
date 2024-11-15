using HassClient.Models;
using HassClient.WS.Messages;
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
        private readonly HassClientWebSocket hassClientWebSocket = new HassClientWebSocket();

        /// <summary>
        /// Api to manage areas in Home Assistant.
        /// </summary>
        public AreasApi Areas { get; }

        /// <summary>
        /// Api to manage categories in Home Assistant.
        /// </summary>
        public CategoriesApi Categories { get; }

        /// <summary>
        /// Api to manage devices in Home Assistant.
        /// </summary>
        public DevicesApi Devices { get; }

        /// <summary>
        /// Api to manage entities in Home Assistant.
        /// </summary>
        public EntitiesEntriesApi Entities { get; }

        /// <summary>
        /// Api to manage floors in Home Assistant.
        /// </summary>
        public FloorsApi Floors { get; }

        /// <summary>
        /// Api to manage labels in Home Assistant.
        /// </summary>
        public LabelsApi Labels { get; }

        /// <summary>
        /// Api to manage services in Home Assistant.
        /// </summary>
        public ServicesApi Services { get; }

        /// <summary>
        /// Api to manage storage entities in Home Assistant.
        /// </summary>
        public StorageEntitiesApi StorageEntities { get; }

        /// <summary>
        /// Api to manage users in Home Assistant.
        /// </summary>
        public UsersApi Users { get; }

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
        /// Gets the <see cref="StateChangedEventListener"/> instance of this client instance.
        /// </summary>
        public StateChangedEventListener StateChangedEventListener { get; private set; }

        /// <summary>
        /// Gets the <see cref="HassClientWebSocket"/> instance of this client instance.
        /// </summary>
        public HassClientWebSocket WebSocket => this.hassClientWebSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="HassWSApi"/> class.
        /// </summary>
        public HassWSApi()
        {
            this.Areas = new AreasApi(this.hassClientWebSocket);
            this.Categories = new CategoriesApi(this.hassClientWebSocket);
            this.Devices = new DevicesApi(this.hassClientWebSocket);
            this.Entities = new EntitiesEntriesApi(this.hassClientWebSocket);
            this.Floors = new FloorsApi(this.hassClientWebSocket);
            this.Labels = new LabelsApi(this.hassClientWebSocket);
            this.Services = new ServicesApi(this.hassClientWebSocket);
            this.StorageEntities = new StorageEntitiesApi(this.hassClientWebSocket);
            this.Users = new UsersApi(this.hassClientWebSocket);
        }

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
        public async Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default)
        {
            await this.hassClientWebSocket.ConnectAsync(connectionParameters, retries, cancellationToken);

            this.StateChangedEventListener = new StateChangedEventListener();
            this.StateChangedEventListener.Initialize(this.hassClientWebSocket);
        }

        /// <summary>
        /// Close the Home Assistant connection as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.CloseAsync(cancellationToken);
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
        /// A task representing the asynchronous operation. The result of the task is the <see cref="ConfigurationModel"/> object.
        /// </returns>
        public Task<ConfigurationModel> GetConfigurationAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetConfigMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<ConfigurationModel>(commandMessage, cancellationToken);
        }

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
        public async Task<IEnumerable<PanelInfo>> ListPanelsAsync(CancellationToken cancellationToken = default)
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
        public Task<IEnumerable<StateModel>> ListStatesAsync(CancellationToken cancellationToken = default)
        {
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<StateModel>>(new GetStatesMessage(), cancellationToken);
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
        public Task<IEnumerable<IntegrationManifest>> ListIntegrationManifestsAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new ListManifestsMessage();
            return this.hassClientWebSocket.SendCommandWithResultAsync<IEnumerable<IntegrationManifest>>(commandMessage, cancellationToken);
        }

        /// <summary>
        /// Gets a collection with the <see cref="EntitySource"/> of the specified entities.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a collection of
        /// <see cref="EntitySource"/> of the specified entities.
        /// </returns>
        public async Task<IEnumerable<EntitySource>> ListEntitySourcesAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new EntitySourceMessage();
            var dict = await this.hassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, EntitySource>>(commandMessage, cancellationToken);
            return dict?.Select(x =>
                {
                    var entitySource = x.Value;
                    entitySource.EntityId = x.Key;
                    return entitySource;
                });
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
        public Task<SearchRelatedResponse> SearchRelatedAsync(ItemTypes itemType, string itemId, CancellationToken cancellationToken = default)
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
