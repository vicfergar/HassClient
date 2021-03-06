using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class AreaRegistryMessagesFactory : StorageCollectionMessagesFactory<Area>
    {
        public static AreaRegistryMessagesFactory Instance = new AreaRegistryMessagesFactory();

        public AreaRegistryMessagesFactory()
            : base("config/area_registry", "area")
        {
        }

        public new BaseOutgoingMessage CreateCreateMessage(Area area)
        {
            return base.CreateCreateMessage(area);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(Area area)
        {
            return base.CreateUpdateMessage(area);
        }

        public new BaseOutgoingMessage CreateDeleteMessage(Area area)
        {
            return base.CreateDeleteMessage(area);
        }
    }
}
