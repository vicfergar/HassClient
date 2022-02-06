using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class DeviceRegistryApiTests : BaseHassWSApiTest
    {
        [Test]
        public async Task GetDevices()
        {
            var devices = await this.hassWSApi.GetDevicesAsync();

            Assert.NotNull(devices);
            Assert.IsNotEmpty(devices);
        }

        [Test]
        public async Task UpdateNameDevice()
        {
            var devices = await this.hassWSApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newName = $"TestDevice_{DateTime.Now.Ticks}";
            testDevice.Name = newName;
            var result = await this.hassWSApi.UpdateDeviceAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newName, testDevice.Name);
        }

        [Test]
        public async Task UpdateAreaIdDevice()
        {
            var devices = await this.hassWSApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newAreaId = $"{DateTime.Now.Ticks}";
            testDevice.AreaId = newAreaId;
            var result = await this.hassWSApi.UpdateDeviceAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newAreaId, testDevice.AreaId);
        }
    }
}
