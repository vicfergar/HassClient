using HassClient.Models;
using HassClient.WS.Messages;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Represents an abstraction layer over <see cref="ClientWebSocket"/> used by
    /// <see cref="IHassWSApi"/> to send commands and subscribe for events.
    /// </summary>
    public interface IHassClientWebSocket : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the client will try to reconnect when connection is lost.
        /// Default: <see langword="true"/>.
        /// </summary>
        bool AutomaticReconnection { get; set; }

        /// <summary>
        /// Gets the current connection state of the web socket.
        /// </summary>
        ConnectionStates ConnectionState { get; }

        /// <summary>
        /// Gets the connected Home Assistant instance version.
        /// </summary>
        CalVer HAVersion { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        bool IsDiposed { get; }

        /// <summary>
        /// Gets a value indicating whether the connection with the server has been
        /// lost and the client is trying to reconnect.
        /// </summary>
        bool IsReconnecting { get; }

        /// <summary>
        /// Gets the number of requests that are pending to be attended by the server.
        /// </summary>
        int PendingRequestsCount { get; }

        /// <summary>
        /// Gets the number of event handler subscriptions.
        /// </summary>
        int SubscriptionsCount { get; }

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> is changed.
        /// </summary>
        event EventHandler<ConnectionStates> ConnectionStateChanged;

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> to an event subscription.
        /// </summary>
        /// <param name="value">The event handler to subscribe.</param>
        /// <param name="eventType">The event type to subscribe to.</param>
        /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The result of the task is a value indicating whether the subscription was successful.
        /// </returns>
        Task<bool> AddEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken);

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
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task ConnectAsync(ConnectionParameters connectionParameters, int retries = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an <see cref="EventHandler{TEventArgs}"/> subscription.
        /// </summary>
        /// <param name="value">The <see cref="EventHandler{TEventArgs}"/> to be removed.</param>
        /// <param name="eventType">The event type filter used in the subscription.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The result of the task is a boolean indicating if the
        /// subscription removal was successfully done.
        /// </returns>
        Task<bool> RemoveEventHandlerSubscriptionAsync(EventHandler<EventResultInfo> value, string eventType, CancellationToken cancellationToken);

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Waits until the client state changed to connected.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        Task<bool> WaitForConnectionAsync(TimeSpan timeout);

        /// <summary>
        /// Waits until the client state changed to connected.
        /// <para>
        /// Either <paramref name="timeout"/> or <paramref name="cancellationToken"/> must be set to avoid never ending wait.
        /// </para>
        /// </summary>
        /// <param name="timeout">The maximum time to wait for connection.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to propagate notification that this operation should be canceled.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The result of the task is <see langword="true"/>
        /// if the client has been connected or <see langword="false"/> if the connection has been closed.
        /// </returns>
        Task<bool> WaitForConnectionAsync(TimeSpan timeout, CancellationToken cancellationToken);
    }
}
