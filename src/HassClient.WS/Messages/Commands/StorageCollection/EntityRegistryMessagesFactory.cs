using HassClient.WS.Messages.Commands;
using HassClient.Models;
using HassClient.Serialization;

namespace HassClient.WS.Messages
{
    internal class EntityRegistryMessagesFactory : StorageCollectionMessagesFactory<EntityRegistryEntry>
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

        public BaseOutgoingMessage CreateUpdateMessage(EntityRegistryEntry entity, string newEntityId, bool? disable, bool forceUpdate)
        {
            var model = this.CreateDefaultUpdateObject(entity, forceUpdate);

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
