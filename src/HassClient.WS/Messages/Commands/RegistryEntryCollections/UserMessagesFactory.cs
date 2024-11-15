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

        public new BaseOutgoingMessage BuildCreateMessage(User user)
        {
            return base.BuildCreateMessage(user);
        }

        public new BaseOutgoingMessage BuildUpdateMessage(User user, bool forceUpdate)
        {
            return base.BuildUpdateMessage(user, forceUpdate);
        }

        public BaseOutgoingMessage BuildDeleteMessage(User user)
        {
            return base.BuildDeleteMessage(user);
        }
    }
}
