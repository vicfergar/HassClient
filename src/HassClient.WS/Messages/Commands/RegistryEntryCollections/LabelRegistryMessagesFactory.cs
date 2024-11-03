using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class LabelRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Label>
    {
        public static LabelRegistryMessagesFactory Instance = new LabelRegistryMessagesFactory();

        public LabelRegistryMessagesFactory()
            : base("config/label_registry", "label")
        {
        }

        public new BaseOutgoingMessage CreateCreateMessage(Label label)
        {
            return base.CreateCreateMessage(label);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(Label label, bool forceUpdate)
        {
            return base.CreateUpdateMessage(label, forceUpdate);
        }

        public BaseOutgoingMessage CreateDeleteMessage(Label label)
        {
            return base.CreateDeleteMessage(label);
        }
    }
}
