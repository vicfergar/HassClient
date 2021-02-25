using HassClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents an <see cref="Entity"/> associated with a <see cref="StorageEntityRegistryEntryBase"/> that
    /// supports committing changes in the configuration.
    /// </summary>
    /// <typeparam name="TStorageEntry">The <see cref="StorageEntityRegistryEntryBase"/> associated with the entity.</typeparam>
    public abstract class CommitableEntity<TStorageEntry> :
        Entity,
        IEditableEntity,
        IReloadableEntity
        where TStorageEntry : StorageEntityRegistryEntryBase
    {
        /// <summary>
        /// The storage registry containing specific configuration parameters for this entity.
        /// </summary>
        protected TStorageEntry SpecificEntityRegistryEntry { get; private set; }

        /// <inheritdoc />
        public virtual bool IsEditable => this.State.GetAttributeValue<bool>("editable");

        /// <summary>
        /// Gets a value indicating that the <see cref="Entity"/> has pending changes waiting to be committed.
        /// </summary>
        public bool HasPendingChanges => this.SpecificEntityRegistryEntry?.HasPendingChanges ?? false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitableEntity{TStorageEntry}"/> class.
        /// </summary>
        /// <param name="hassInstance">The <see cref="HassInstance"/> associated with this entity.</param>
        /// <param name="entityDefinition">The entity definition.</param>
        protected CommitableEntity(HassInstance hassInstance, EntityDefinition entityDefinition)
            : base(hassInstance, entityDefinition)
        {
            this.SpecificEntityRegistryEntry = (TStorageEntry)entityDefinition.SpecificEntityRegistryEntry;
        }

        /// <summary>
        /// Commits pending changes from the <see cref="SpecificEntityRegistryEntry"/>.
        /// </summary>
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
        public Task<bool> CommitChangesAsync(bool forceUpdate = false, CancellationToken cancellationToken = default)
        {
            if (this.SpecificEntityRegistryEntry == null)
            {
                throw new InvalidOperationException($"{nameof(this.CommitChangesAsync)} cannot be used for non editable entities.");
            }

            return this.hassInstance.HassWSApi.UpdateStorageEntityRegistryEntryAsync(this.SpecificEntityRegistryEntry, forceUpdate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ReloadAsync(CancellationToken cancellationToken = default)
        {
            return this.CallServiceAsync(KnownServices.Reload, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Looks for an specific property within the <see cref="SpecificEntityRegistryEntry"/>. If no registry entry is available
        /// an attribute value will be returned as fallback.
        /// </summary>
        /// <typeparam name="T">The type of the property or attribute.</typeparam>
        /// <param name="attributeName">The name of the attribute used as fallback value.</param>
        /// <param name="propertyGetter">A function returning the expected property value.</param>
        /// <returns>The specific property value or the attribute as fallback.</returns>
        protected T GetPropertyOrFallbackAttribute<T>(string attributeName, Func<TStorageEntry, T> propertyGetter)
        {
            if (this.SpecificEntityRegistryEntry != null)
            {
                return propertyGetter(this.SpecificEntityRegistryEntry);
            }

            return this.State.GetAttributeValue<T>(attributeName);
        }

        /// <summary>
        /// Looks for an specific property within the <see cref="SpecificEntityRegistryEntry"/>. If no registry entry is available
        /// an attribute value will be returned as fallback.
        /// </summary>
        /// <typeparam name="TIn">The type of the property or attribute before been converted.</typeparam>
        /// <typeparam name="TOut">The type of the property or attribute once converted.</typeparam>
        /// <param name="attributeName">The name of the attribute used as fallback value.</param>
        /// <param name="propertyGetter">A function returning the expected property value.</param>
        /// <param name="converter">A function that converts the value from <typeparamref name="TIn"/> to <typeparamref name="TOut"/>.</param>
        /// <returns>The specific property value or the attribute as fallback.</returns>
        protected TOut GetPropertyOrFallbackAttribute<TIn, TOut>(string attributeName, Func<TStorageEntry, TIn> propertyGetter, Func<TIn, TOut> converter)
        {
            TIn unconverted = this.SpecificEntityRegistryEntry != null ?
                                propertyGetter(this.SpecificEntityRegistryEntry) :
                                this.State.GetAttributeValue<TIn>(attributeName);

            return converter(unconverted);
        }
    }
}
