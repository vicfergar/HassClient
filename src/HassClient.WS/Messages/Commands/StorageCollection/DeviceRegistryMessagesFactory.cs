using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class DeviceRegistryMessagesFactory : StorageCollectionMessagesFactory
    {
        public static DeviceRegistryMessagesFactory Instance = new DeviceRegistryMessagesFactory();

        public DeviceRegistryMessagesFactory()
            : base("config/device_registry", "device")
        {
        }

        public BaseOutgoingMessage CreateUpdateMessage(Device device, bool? disable)
        {
            var selectedProperties = new[] { nameof(Device.AreaId), nameof(Device.nameByUser) };

            var model = HassSerializer.CreateJObject(device, selectedProperties);

            if (disable.HasValue)
            {
                var merged = HassSerializer.CreateJObject(new { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
                model.Merge(merged);
            }

            return this.CreateUpdateMessage(device.Id, model);
        }
    }
}
