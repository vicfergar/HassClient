using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    internal class UserStorageCollectionCommandProcessor
        : RegistryEntryCollectionCommandProcessor<UserMessagesFactory, User>
    {
        protected override object ProccessCreateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var user = (User)base.ProccessCreateCommand(context, merged);
            user.SetIsActive(true);
            return new UserResponse() { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
        }

        protected override object ProccessUpdateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var user = base.ProccessUpdateCommand(context, merged);
            return new UserResponse() { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
        }

        protected override object ProccessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return base.ProccessListCommand(context, merged);
        }

        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            var ownerUser = User.CreateUnmodified(this.faker.RandomUUID(), "owner", true);
            context.HassDB.CreateObject(ownerUser);
        }
    }
}
