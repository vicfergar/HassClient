using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class DeviceRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Device>
    {
        public static DeviceRegistryMessagesFactory Instance = new DeviceRegistryMessagesFactory();

        public DeviceRegistryMessagesFactory()
            : base("config/device_registry", "device")
        {
        }

        public BaseOutgoingMessage BuildUpdateMessage(Device device, bool? disable, bool forceUpdate)
        {
            var model = this.BuildDefaultUpdateObject(device, forceUpdate);

            if (disable.HasValue)
            {
                var merged = HassSerializer.CreateJObject(new { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
                model.Merge(merged);
            }

            return this.BuildUpdateMessage(device.Id, model);
        }
    }
}
