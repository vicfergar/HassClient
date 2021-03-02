using HassClient.Models;
using HassClient.Serialization;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(DeviceRegistryTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(DeviceRegistryTests) + "WithRealServer")]
    public class DeviceRegistryTests : BaseHassWSApiTest
    {
        public DeviceRegistryTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public async Task GetDevices()
        {
            var devices = await this.hassWSApi.GetDevicesAsync();

            Assert.NotNull(devices);
            Assert.IsNotEmpty(devices);
        }

        [Test]
        public void DeserializedDevicesHasNoPendingChanges()
        {
            var testDevice = HassSerializer.DeserializeObject<Device>("{name:\"test\"}");
            Assert.IsFalse(testDevice.HasPendingChanges);
        }

        [Test]
        public void NewDeviceHasNoPendingChanges()
        {
            var testDevice = new Device(nameof(Device.Id), nameof(Device.Name));
            Assert.IsFalse(testDevice.HasPendingChanges);
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testDevice = new Device(nameof(Device.Id), nameof(Device.Name));

            testDevice.Name = $"TestDevice_{DateTime.Now.Ticks}";
            Assert.IsTrue(testDevice.HasPendingChanges);

            testDevice.Name = null;
            Assert.False(testDevice.HasPendingChanges);
        }

        [Test]
        public void SetNewAreaIdMakesHasPendingChangesTrue()
        {
            var testDevice = new Device(nameof(Device.Id), nameof(Device.Name));

            testDevice.AreaId = $"TestDevice_{DateTime.Now.Ticks}";
            Assert.IsTrue(testDevice.HasPendingChanges);

            testDevice.AreaId = null;
            Assert.False(testDevice.HasPendingChanges);
        }

        [Test]
        public void NameIsNameByUserIfDefined()
        {
            var testDevice = new Device(nameof(Device.Id), nameof(Device.Name));

            Assert.AreEqual(testDevice.OriginalName, testDevice.Name);

            var testName = $"TestDevice_{DateTime.Now.Ticks}";
            testDevice.Name = testName;
            Assert.AreEqual(testName, testDevice.Name);

            testDevice.Name = null;
            Assert.AreEqual(testDevice.OriginalName, testDevice.Name);
        }

        [Test]
        public async Task UpdateDevice()
        {
            var devices = await this.hassWSApi.GetDevicesAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            testDevice.Name = $"TestDevice_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateDeviceAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
        }
    }
}
