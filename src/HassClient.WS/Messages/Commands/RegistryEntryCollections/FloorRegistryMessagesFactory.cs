using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class FloorRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Floor>
    {
        public static FloorRegistryMessagesFactory Instance = new FloorRegistryMessagesFactory();

        public FloorRegistryMessagesFactory()
            : base("config/floor_registry", "floor")
        {
        }

        public new BaseOutgoingMessage BuildCreateMessage(Floor floor)
        {
            return base.BuildCreateMessage(floor);
        }

        public new BaseOutgoingMessage BuildUpdateMessage(Floor floor, bool forceUpdate)
        {
            return base.BuildUpdateMessage(floor, forceUpdate);
        }

        public BaseOutgoingMessage BuildDeleteMessage(Floor floor)
        {
            return base.BuildDeleteMessage(floor);
        }
    }
}
