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

        public BaseOutgoingMessage BuildGetMessage(string entityId)
        {
            return this.BuildCustomOperationMessage("get", entityId);
        }

        public BaseOutgoingMessage BuildUpdateMessage(EntityRegistryEntry entity, string newEntityId, bool? disable, bool forceUpdate)
        {
            var shouldForceUpdate = !entity.SupportsPartialUpdates || forceUpdate;
            var model = this.BuildDefaultUpdateObject(entity, shouldForceUpdate);

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

            return this.BuildUpdateMessage(entity.EntityId, model);
        }

        public BaseOutgoingMessage BuildDeleteMessage(EntityRegistryEntry entity)
        {
            return this.BuildCustomOperationMessage("remove", entity.EntityId);
        }
    }
}
