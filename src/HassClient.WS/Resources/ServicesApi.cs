using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS
{
    /// <summary>
    /// Api to manage services in Home Assistant.
    /// </summary>
    public class ServicesApi : ResourceApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesApi"/> class.
        /// </summary>
        /// <param name="webSocket">The websocket client instance.</param>
        public ServicesApi(HassClientWebSocket webSocket)
            : base(webSocket)
        {
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
        public async Task<IEnumerable<ServiceDomain>> ListAsync(CancellationToken cancellationToken = default)
        {
            var commandMessage = new GetServicesMessage();
            var dict = await this.HassClientWebSocket.SendCommandWithResultAsync<Dictionary<string, JRaw>>(commandMessage, cancellationToken);
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
        public async Task<Context> CallAsync(string domain, string service, object data = null, CancellationToken cancellationToken = default)
        {
            var commandMessage = new CallServiceMessage(domain, service, data);
            var state = await this.HassClientWebSocket.SendCommandWithResultAsync<StateModel>(commandMessage, cancellationToken);
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
        public async Task<bool> CallAsync(KnownDomains domain, KnownServices service, object data = null, CancellationToken cancellationToken = default)
        {
            var context = await this.CallAsync(domain.ToDomainString(), service.ToServiceString(), data, cancellationToken);
            return context != null;
        }

        /// <summary>
        /// Calls a service within a specific domain targeting specific entities.
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
        public Task<bool> CallForEntitiesAsync(string domain, string service, params string[] entityIds)
        {
            return this.CallForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

        /// <summary>
        /// Calls a service within a specific domain targeting specific entities.
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
        public async Task<bool> CallForEntitiesAsync(string domain, string service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            var context = await this.CallAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
            return context != null;
        }

        /// <summary>
        /// Calls a service within a specific domain targeting specific entities.
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
        public Task<bool> CallForEntitiesAsync(KnownDomains domain, KnownServices service, params string[] entityIds)
        {
            return this.CallForEntitiesAsync(domain, service, CancellationToken.None, entityIds);
        }

        /// <summary>
        /// Calls a service within a specific domain targeting specific entities.
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
        public Task<bool> CallForEntitiesAsync(KnownDomains domain, KnownServices service, CancellationToken cancellationToken = default, params string[] entityIds)
        {
            return this.CallAsync(domain, service, new { entity_id = entityIds }, cancellationToken);
        }
    }
}
