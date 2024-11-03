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

        public new BaseOutgoingMessage CreateCreateMessage(Floor floor)
        {
            return base.CreateCreateMessage(floor);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(Floor floor, bool forceUpdate)
        {
            return base.CreateUpdateMessage(floor, forceUpdate);
        }

        public BaseOutgoingMessage CreateDeleteMessage(Floor floor)
        {
            return base.CreateDeleteMessage(floor);
        }
    }
}
