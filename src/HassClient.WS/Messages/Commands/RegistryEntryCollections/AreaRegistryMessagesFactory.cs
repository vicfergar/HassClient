using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class AreaRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Area>
    {
        public static AreaRegistryMessagesFactory Instance = new AreaRegistryMessagesFactory();

        public AreaRegistryMessagesFactory()
            : base("config/area_registry", "area")
        {
        }

        public new BaseOutgoingMessage BuildCreateMessage(Area area)
        {
            return base.BuildCreateMessage(area);
        }

        public new BaseOutgoingMessage BuildUpdateMessage(Area area, bool forceUpdate)
        {
            return base.BuildUpdateMessage(area, forceUpdate);
        }

        public BaseOutgoingMessage BuildDeleteMessage(Area area)
        {
            return base.BuildDeleteMessage(area);
        }
    }
}
