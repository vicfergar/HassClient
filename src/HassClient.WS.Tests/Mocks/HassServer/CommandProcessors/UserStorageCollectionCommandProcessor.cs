using HassClient.Models;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    internal class UserStorageCollectionCommandProcessor
        : StorageCollectionCommandProcessor<UserMessagesFactory, User>
    {
        protected override object ProccessCreateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var user = (User)base.ProccessCreateCommand(context, merged);
            user.SetIsActive(true);
            return new UserResponse() { User = user };
        }

        protected override IEnumerable<User> ProccessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return base.ProccessListCommand(context, merged);
        }

        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            var ownerUser = new User("owner", true);
            context.HassDB.CreateObject(ownerUser);
        }
    }
}
