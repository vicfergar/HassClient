using HassClient.WS.Messages.Commands;
using HassClient.Models;

namespace HassClient.WS.Messages
{
    internal class InputBooleanMessagesFactory : StorageCollectionMessagesFactory<InputBoolean>
    {
        public static InputBooleanMessagesFactory Instance = new InputBooleanMessagesFactory();

        public InputBooleanMessagesFactory()
            : base("input_boolean", "input_boolean")
        {
        }

        public new BaseOutgoingMessage CreateCreateMessage(InputBoolean inputBoolean)
        {
            return base.CreateCreateMessage(inputBoolean);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(InputBoolean inputBoolean, bool forceUpdate)
        {
            return base.CreateUpdateMessage(inputBoolean, forceUpdate);
        }

        public new BaseOutgoingMessage CreateDeleteMessage(InputBoolean inputBoolean)
        {
            return base.CreateDeleteMessage(inputBoolean);
        }
    }
}
