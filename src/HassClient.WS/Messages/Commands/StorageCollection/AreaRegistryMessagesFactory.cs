using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class AreaRegistryMessagesFactory : StorageCollectionMessagesFactory
    {
        public static AreaRegistryMessagesFactory Instance = new AreaRegistryMessagesFactory();

        public AreaRegistryMessagesFactory()
            : base("config/area_registry", "area")
        {
        }

        public BaseOutgoingMessage CreateCreateMessage(string name)
        {
            return this.CreateCreateMessage(new { Name = name }, null);
        }

        public BaseOutgoingMessage CreateUpdateMessage(Area area)
        {
            var selectedProperties = new[] { nameof(Area.Name) };
            return this.CreateUpdateMessage(area.Id, area, selectedProperties);
        }

        public BaseOutgoingMessage CreateDeleteMessage(Area area)
        {
            return this.CreateDeleteMessage(area.Id);
        }
    }
}
