using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HassClient.WS.Messages.Commands
{
    /// <summary>
    /// Factory used to create Registry Entry Collection Messages.
    /// </summary>
    /// <typeparam name="TModel">The modifiable model type associated with the Storage Collection.</typeparam>
    public abstract class RegistryEntryCollectionMessagesFactory<TModel>
        where TModel : ModifiableModelBase
    {
        private readonly string apiPrefix;

        private readonly string modelName;

        /// <summary>
        /// Gets the API prefix used in underlying message types.
        /// </summary>
        public string ApiPrefix => this.apiPrefix;

        /// <summary>
        /// Gets the model name used to generate model identifier property.
        /// </summary>
        public string ModelName => this.modelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryEntryCollectionMessagesFactory{TModel}"/> class.
        /// </summary>
        /// <param name="apiPrefix">The API prefix used in underlying message types.</param>
        /// <param name="modelName">The name used to generate model identifier property.</param>
        protected RegistryEntryCollectionMessagesFactory(string apiPrefix, string modelName)
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
             */

            this.apiPrefix = apiPrefix;
            this.modelName = modelName;
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to request the list of registered items.
        /// </summary>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to request the list of registered items.
        /// </returns>
        public BaseOutgoingMessage BuildListMessage()
        {
            return this.BuildListMessage(mergedObject: null);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </summary>
        /// <param name="mergedObject">Object containing additional fields that will be merged to the create message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildListMessage(object mergedObject = null)
        {
            return new RawCommandMessage($"{this.apiPrefix}/list", mergedObject);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to add a new item in the collection
        /// registry filtering modifiable properties only.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildCreateMessage(TModel model)
        {
            return this.BuildCreateMessage(this.BuildDefaultCreateObject(model));
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to update an existing item from
        /// the collection registry filtering modified properties only.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property.
        /// </param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildUpdateMessage(TModel model, bool forceUpdate)
        {
            return this.BuildUpdateMessage(model.UniqueId, this.BuildDefaultUpdateObject(model, forceUpdate));
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </summary>
        /// <param name="model">The model for the generation of the message.</param>
        /// <param name="mergedObject">Object containing additional fields that will be merged to the delete message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildDeleteMessage(TModel model, object mergedObject = null)
        {
            return this.BuildDeleteMessage(model.UniqueId, mergedObject);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </summary>
        /// <param name="model">The object model to be added.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to add a new item in the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildCreateMessage(object model, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = HassSerializer.CreateJObject(model, selectedProperties);
            return new RawCommandMessage($"{this.apiPrefix}/create", mergedObject);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </summary>
        /// <param name="modelId">The unique identifier of the collection registry item to update.</param>
        /// <param name="model">The object model to be updated.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to update an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildUpdateMessage(string modelId, object model, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = HassSerializer.CreateJObject(model, selectedProperties);
            this.AddModelIdProperty(mergedObject, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/update", mergedObject);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </summary>
        /// <param name="modelId">The unique identifier of the collection registry item to delete.</param>
        /// <param name="mergedObject">Object containing additional fields that will be merged to the delete message.</param>
        /// <returns>
        /// A <see cref="BaseOutgoingMessage"/> used to delete an existing item from the collection registry.
        /// </returns>
        protected BaseOutgoingMessage BuildDeleteMessage(string modelId, object mergedObject = null)
        {
            var mergedObjectWithModelId = mergedObject != null ? HassSerializer.CreateJObject(mergedObject) : new JObject();
            this.AddModelIdProperty(mergedObjectWithModelId, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/delete", mergedObjectWithModelId);
        }

        /// <summary>
        /// Builds a <see cref="BaseOutgoingMessage"/> used in specific operations for certain collection registry items.
        /// </summary>
        /// <param name="customOpName">The custom operation name.</param>
        /// <param name="modelId">The unique identifier of the collection registry item.</param>
        /// <param name="model">The object model involved in the operation, if any.</param>
        /// <param name="selectedProperties">White-list containing the name of the properties to extract from the <paramref name="model"/> object.
        /// When <see langword="null"/>, no filter will be applied.</param>
        /// <returns>A <see cref="BaseOutgoingMessage"/> used in specific operations for certain collection registry items.</returns>
        protected BaseOutgoingMessage BuildCustomOperationMessage(string customOpName, string modelId, TModel model = null, IEnumerable<string> selectedProperties = null)
        {
            var mergedObject = model != null ? HassSerializer.CreateJObject(model, selectedProperties) : new JObject();
            this.AddModelIdProperty(mergedObject, modelId);
            return new RawCommandMessage($"{this.apiPrefix}/{customOpName}", mergedObject);
        }

        /// <summary>
        /// Builds the default create object filtering modifiable property names only.
        /// </summary>
        /// <param name="model">The object model to be updated.</param>
        /// <returns>The default create object filtering modifiable property names only.</returns>
        protected JObject BuildDefaultCreateObject(TModel model)
        {
            return HassSerializer.CreateJObject(model, model.GetModifiablePropertyNames());
        }

        /// <summary>
        /// Builds the default update object filtering modified property names only.
        /// </summary>
        /// <param name="model">The object model to be updated.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property.
        /// </param>
        /// <returns>The default update object filtering modified property names only.</returns>
        protected JObject BuildDefaultUpdateObject(TModel model, bool forceUpdate)
        {
            var selectedProperties = forceUpdate ? model.GetModifiablePropertyNames() : model.GetModifiedPropertyNames();
            return HassSerializer.CreateJObject(model, selectedProperties);
        }

        private void AddModelIdProperty(JObject mergedObject, string modelId)
        {
            var propertyName = $"{this.modelName}_id";
            if (!mergedObject.ContainsKey(propertyName))
            {
                mergedObject.Add(propertyName, modelId);
            }
        }
    }
}
