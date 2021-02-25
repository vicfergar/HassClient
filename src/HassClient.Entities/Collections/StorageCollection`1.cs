using HassClient.Models;
using System.Collections.Generic;

namespace HassClient.Entities.Collections
{
    /// <summary>
    /// Represents collection to store <see cref="RegistryEntryBase"/> entries.
    /// </summary>
    /// <typeparam name="TRegistryEntry">A type that inherits from <see cref="RegistryEntryBase"/>.</typeparam>
    public class StorageCollection<TRegistryEntry>
        : HassCollection<TRegistryEntry>
        where TRegistryEntry : RegistryEntryBase
    {
        private readonly HashSet<TRegistryEntry> dirtyEntries = new HashSet<TRegistryEntry>();

        /// <summary>
        /// Gets a collection with the entries in dirty state.
        /// </summary>
        public IEnumerable<TRegistryEntry> DirtyEntries => this.dirtyEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollection{TRegistryEntry}"/> class.
        /// </summary>
        /// <param name="values">
        /// Initial values for the <see cref="StorageCollection{TRegistryEntry}"/>.
        /// </param>
        public StorageCollection(IEnumerable<TRegistryEntry> values)
            : base()
        {
            foreach (var entry in values)
            {
                this.Add(entry.UniqueId, entry);
            }
        }

        /// <summary>
        /// Mark as dirty all the entries contained in the collection.
        /// </summary>
        public void MarkAllEntriesAsDirty()
        {
            foreach (var entry in this.Values)
            {
                this.dirtyEntries.Add(entry);
                entry.IsDirty = true;
            }
        }

        /// <summary>
        /// Marks as dirty the entry with the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to update.</param>
        public void MarkAsDirty(string key)
        {
            this.TryGetValue(key, out var entry);
            this.dirtyEntries.Add(entry);
            entry.IsDirty = true;
        }

        /// <summary>
        /// Clear the dirty flag for every entry contained in the collection.
        /// </summary>
        public void ClearDirtyEntries()
        {
            foreach (var entry in this.dirtyEntries)
            {
                entry.IsDirty = false;
            }

            this.dirtyEntries.Clear();
        }
    }
}
