using HassClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents an input boolean.
    /// </summary>
    public abstract class StorageEntityRegistryEntryBase : NamedEntryBase, IEntityEntry
    {
        private KnownDomains domain;

        /// <inheritdoc />
        internal protected override string UniqueId
        {
            get => this.Id;
            set => this.Id = value;
        }

        /// <summary>
        /// Gets the entity identifier of the entity entry.
        /// </summary>
        [JsonProperty]
        public string Id { get; protected set; }

        /// <inheritdoc />
        public string EntityId => $"{this.domain.ToDomainString()}.{this.UniqueId}";

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageEntityRegistryEntryBase"/> class.
        /// </summary>
        protected StorageEntityRegistryEntryBase()
        {
            this.domain = GetDomain(this.GetType());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageEntityRegistryEntryBase"/> class.
        /// </summary>
        /// <param name="name">The entity name.</param>
        /// <param name="icon">The entity icon.</param>
        protected StorageEntityRegistryEntryBase(string name, string icon)
            : base(name, icon)
        {
            this.domain = GetDomain(this.GetType());
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.domain}: {this.Name}";

        private static Dictionary<Type, KnownDomains> domainsByType = new Dictionary<Type, KnownDomains>();

        internal static KnownDomains GetDomain<T>()
            where T : StorageEntityRegistryEntryBase
        {
            return GetDomain(typeof(T));
        }

        internal static KnownDomains GetDomain(Type type)
        {
            if (!domainsByType.TryGetValue(type, out var domain))
            {
                var attribute = (StorageEntityDomainAttribute)Attribute.GetCustomAttribute(type, typeof(StorageEntityDomainAttribute));
                domain = attribute.Domain;
                domainsByType.Add(type, domain);
            }

            return domain;
        }
    }
}
