using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a service domain definition in Home Assistant.
    /// </summary>
    public class ServiceDomain
    {
        /// <summary>
        /// Gets or sets the service domain's name.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the list of services in this domain.
        /// </summary>
        public Dictionary<string, Service> Services { get; set; }

        /// <summary>
        /// Gets a flat, fully-qualified list of services in this service domain.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<string> FlatServiceList => this.Services.Select(s => $"{this.Domain}.{s}");

        /// <summary>
        /// Retrieves a service object from this domain by its name, or returns <see langword="null" /> if the service does not exist.
        /// </summary>
        /// <param name="service">The service name to retrieve.</param>
        /// <returns>The <see cref="Service" />, if the name exists in this domain, otherwise <see langword="null" />.</returns>
        [JsonIgnore]
        public Service this[string service] => this.Services.ContainsKey(service) ? this.Services[service] : null;

        /// <summary>
        /// If the specified <paramref name="serviceName" /> exists, populates the <paramref name="fullServiceName" /> with the
        /// fully qualified service name (domain.service), and returns <see langword="true" />. Otherwise, if the service does not exist,
        /// returns <see langword="false" />.
        /// </summary>
        /// <param name="serviceName">The relative name of the service to look up in this service domain.</param>
        /// <param name="fullServiceName">Upon successful match, will be set to the fullly qualified service name (domain.service). Otherwise, <see langword="null" />.</param>
        /// <returns><see langword="true" /> if a match was found, otherwise <see langword="false" />.</returns>
        public bool TryGetService(string serviceName, out string fullServiceName)
        {
            if (this.Services.ContainsKey(serviceName))
            {
                fullServiceName = $"{this.Domain}.{serviceName}";
                return true;
            }

            fullServiceName = null;
            return false;
        }

        /// <inheritdoc />
        public override string ToString() => $"Service Domain: {this.Domain} ({this.Services?.Count ?? 0} service(s))";
    }
}
