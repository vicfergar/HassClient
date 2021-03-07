using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HassClient.WS.Messages.Commands
{
    /// <summary>
    /// Factory used to create Storage Collection Messages.
    /// </summary>
    /// <typeparam name="TModel">The modifiable model type associated with the Storage Collection.</typeparam>
    public abstract class StorageCollectionMessagesFactory<TModel>
        where TModel : RegistryEntryBase
    {
        private readonly string apiPrefix;

        private readonly string modelName;

        /// <summary>
        /// Gets the API prefix used in underlaying message types.
        /// </summary>
        public string ApiPrefix => this.apiPrefix;

        /// <summary>
        /// Gets the model name used to generate model identifier property.
        /// </summary>
        public string ModelName => this.modelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollectionMessagesFactory{TModel}"/> class.
        /// </summary>
        /// <param name="apiPrefix">The API prefix used in underlaying message types.</param>
        /// <param name="modelName">The name used to generate model identifier property.</param>
        protected StorageCollectionMessagesFactory(string apiPrefix, string modelName)
        {
            if (string.IsNullOrEmpty(apiPrefix))
            {
                throw new ArgumentException($"'{nameof(apiPrefix)}' cannot be null or empty", nameof(apiPrefix));
            }

            if (string.IsNullOrEmpty(modelName))
            {
                throw new ArgumentException($"'{nameof(modelName)}' cannot be null or empty", nameof(modelName));
            }

            /* TODO: Implement other found API prefixes:
             * "lovelace/resources"
             * "lovelace/dashboards"
             * "tag"
             * "image"
             * "counter"
             * "input_number"
             * "input_select"
             * "input_text
             * "zone"
             * "timer"
             * "input_datetime"
             * "person"
             */

            this.apiPrefix = apiPrefix;
            this.modelName = modelName;
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to request the list of registered items.
        /// </summary>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to request the list of registered items.
        /// </returns>
        public BaseOutgoingMessage CreateListMessage()
        {
            return new RawCommandMessage($"{this.apiPrefix}/list");
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to add a new item in the collection
        /// registry filtering modifiable properties only.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateCreateMessage(TModel model)
        {
            return this.CreateCreateMessage(this.CreateDefaultCreateObject(model));
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to update an existing item from
        /// the collection registry filtering modified properties only.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property.
        /// </param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateUpdateMessage(TModel model, bool forceUpdate)
        {
            return this.CreateUpdateMessage(model.UniqueId, this.CreateDefaultUpdateObject(model, forceUpdate));
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateDeleteMessage(TModel model)
        {
            return this.CreateDeleteMessage(model.UniqueId);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </summary>
        /// <param name="model">The object model to be added.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateCreateMessage(object model, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = HassSerializer.CreateJObject(model, selectedProperties);
            return new RawCommandMessage($"{this.apiPrefix}/create", mergedObject);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </summary>
        /// <param name="modelId">The unique identifier of the collection registry item to update.</param>
        /// <param name="model">The object model to be updated.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateUpdateMessage(string modelId, object model, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = HassSerializer.CreateJObject(model, selectedProperties);
            this.AddModelIdProperty(mergedObject, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/update", mergedObject);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </summary>
        /// <param name="modelId">The unique identifier of the collection registry item to delete.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage CreateDeleteMessage(string modelId)
        {
            var mergedObject = new JObject();
            this.AddModelIdProperty(mergedObject, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/delete", mergedObject);
        }

        /// <summary>
        /// Creates a <see cref="BaseOutgoingMessage"/> used in specific operations for certain collection registry items.
        /// </summary>
        /// <param name="customOpName">The custom operation name.</param>
        /// <param name="modelId">The unique identifier of the collection registry item.</param>
        /// <param name="model">The object model involved in the operation, if any.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>A <see cref="BaseOutgoingMessage"/> used in specific operations for certain collection registry items.</returns>
        protected BaseOutgoingMessage CreateCustomOperationMessage(string customOpName, string modelId, TModel model = null, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = model != null ? HassSerializer.CreateJObject(model, selectedProperties) : new JObject();
            this.AddModelIdProperty(mergedObject, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/{customOpName}", mergedObject);
        }

        /// <summary>
        /// Creates the default create object filtering modifiable property names only.
        /// </summary>
        /// <param name="model">The object model to be updated.</param>
        /// <returns>The default create object filtering modifiable property names only.</returns>
        protected JObject CreateDefaultCreateObject(TModel model)
        {
            return HassSerializer.CreateJObject(model, model.GetModifiablePropertyNames());
        }

        /// <summary>
        /// Creates the default update object filtering modified property names only.
        /// </summary>
        /// <param name="model">The object model to be updated.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property.
        /// </param>
        /// <returns>The default update object filtering modified property names only.</returns>
        protected JObject CreateDefaultUpdateObject(TModel model, bool forceUpdate)
        {
            var selectedProperties = forceUpdate ? model.GetModifiablePropertyNames() : model.GetModifiedPropertyNames();
            return HassSerializer.CreateJObject(model, selectedProperties);
        }

        private void AddModelIdProperty(JObject mergedObject, string modelId)
        {
            mergedObject.TryAdd($"{this.modelName}_id", modelId);
        }
    }
}
