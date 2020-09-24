using HassClient.Net.ClientWebSocket.Messages.Commands;

namespace HassClient.Net.WSMessages
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

        public BaseOutgoingMessage CreateUpdateMessage(Models.Area area)
        {
            return this.CreateUpdateMessage(area.Id, area, new[] { nameof(area.Name) });
        }

        public BaseOutgoingMessage CreateDeleteMessage(Models.Area area)
        {
            return this.CreateDeleteMessage(area.Id);
        }
    }
}
