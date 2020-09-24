using HassClient.Net.ClientWebSocket.Messages.Commands;
using HassClient.Net.Models;

namespace HassClient.Net.WSMessages
{
    internal class InputBooleanMessagesFactory : StorageCollectionMessagesFactory
    {
        public static InputBooleanMessagesFactory Instance = new InputBooleanMessagesFactory();

        public InputBooleanMessagesFactory()
            : base("input_boolean", "input_boolean")
        {
        }

        public BaseOutgoingMessage CreateCreateMessage(InputBoolean inputBoolean)
        {
            var selectedProperties = new[] { nameof(InputBoolean.Name), nameof(InputBoolean.Initial), nameof(InputBoolean.Icon) };
            return this.CreateCreateMessage(inputBoolean, selectedProperties);
        }

        public BaseOutgoingMessage CreateUpdateMessage(InputBoolean inputBoolean)
        {
            var selectedProperties = new[] { nameof(InputBoolean.Name), nameof(InputBoolean.Initial), nameof(InputBoolean.Icon) };
            return this.CreateUpdateMessage(inputBoolean.UniqueId, inputBoolean, selectedProperties);
        }

        public BaseOutgoingMessage CreateDeleteMessage(InputBoolean inputBoolean)
        {
            return this.CreateDeleteMessage(inputBoolean.UniqueId);
        }
    }
}
