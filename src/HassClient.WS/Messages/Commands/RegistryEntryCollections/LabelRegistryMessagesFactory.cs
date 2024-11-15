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

        public new BaseOutgoingMessage BuildCreateMessage(Label label)
        {
            return base.BuildCreateMessage(label);
        }

        public new BaseOutgoingMessage BuildUpdateMessage(Label label, bool forceUpdate)
        {
            return base.BuildUpdateMessage(label, forceUpdate);
        }

        public BaseOutgoingMessage BuildDeleteMessage(Label label)
        {
            return base.BuildDeleteMessage(label);
        }
    }
}
