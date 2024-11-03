using Newtonsoft.Json;
using System;

namespace HassClient.Models
{
    /// <summary>
    /// Base class that defines a registry entry.
    /// </summary>
    public abstract class RegistryEntryBase : NamedEntryBase, ITimeTracked
    {
        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset ModifiedAt { get; private set; }

        [JsonConstructor]
        private protected RegistryEntryBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryEntryBase"/> class.
        /// </summary>
        /// <param name="name">The registry entry name.</param>
        /// <param name="icon">The registry entry icon.</param>
        protected RegistryEntryBase(string name, string icon)
            : base(name, icon)
        {
        }
    }
}
