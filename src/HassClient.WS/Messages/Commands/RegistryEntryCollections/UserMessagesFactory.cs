using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class UserMessagesFactory : RegistryEntryCollectionMessagesFactory<User>
    {
        public static UserMessagesFactory Instance = new UserMessagesFactory();

        public UserMessagesFactory()
            : base("config/auth", "user")
        {
        }

        public new BaseOutgoingMessage CreateCreateMessage(User user)
        {
            return base.CreateCreateMessage(user);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(User user, bool forceUpdate)
        {
            return base.CreateUpdateMessage(user, forceUpdate);
        }

        public BaseOutgoingMessage CreateDeleteMessage(User user)
        {
            return base.CreateDeleteMessage(user);
        }
    }
}
