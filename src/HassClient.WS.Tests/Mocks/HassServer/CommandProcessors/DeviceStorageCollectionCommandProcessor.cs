using HassClient.Models;
using HassClient.WS.Messages;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    internal class DeviceStorageCollectionCommandProcessor
        : RegistryEntryCollectionCommandProcessor<DeviceRegistryMessagesFactory, Device>
    {
        protected override void PrepareHassContext(MockHassServerRequestContext context)
        {
            base.PrepareHassContext(context);
            context.HassDB.CreateObject(MockHassModelFactory.DeviceFaker.Generate());
        }
    }
}
