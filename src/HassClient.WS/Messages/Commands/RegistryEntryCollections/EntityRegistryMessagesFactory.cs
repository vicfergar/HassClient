using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class EntityRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<EntityRegistryEntry>
    {
        public static EntityRegistryMessagesFactory Instance = new EntityRegistryMessagesFactory();

        public EntityRegistryMessagesFactory()
            : base("config/entity_registry", "entity")
        {
        }

        public BaseOutgoingMessage CreateGetMessage(string entityId)
        {
            return this.CreateCustomOperationMessage("get", entityId);
        }

        /// <summary>
        /// Creates a message to update an existing <see cref="EntityRegistryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="EntityRegistryEntry"/> to be updated.</param>
        /// <param name="newEntityId">If not <see langword="null"/>, it will update the current entity id.</param>
        /// <param name="disable">If not <see langword="null"/>, it will enable or disable the entity.</param>
        /// <param name="forceUpdate">
        /// Indicates if the update message force the update of every modifiable property. If the entity does not
        /// support partial updates, this parameter is ignored.
        /// </param>
        /// <returns>The message to update an existing <see cref="EntityRegistryEntry"/>.</returns>
        public BaseOutgoingMessage CreateUpdateMessage(EntityRegistryEntry entity, string newEntityId, bool? disable, bool forceUpdate)
        {
            var shouldForceUpdate = !entity.SupportsPartialUpdates || forceUpdate;
            var model = this.CreateDefaultUpdateObject(entity, shouldForceUpdate);

            if (newEntityId != null)
            {
                var merged = HassSerializer.CreateJObject(new { NewEntityId = newEntityId });
                model.Merge(merged);
            }

            if (disable.HasValue)
            {
                var merged = HassSerializer.CreateJObject(new { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
                model.Merge(merged);
            }

            return this.CreateUpdateMessage(entity.EntityId, model);
        }

        public new BaseOutgoingMessage CreateDeleteMessage(EntityRegistryEntry entity)
        {
            return this.CreateCustomOperationMessage("remove", entity.EntityId);
        }
    }
}
