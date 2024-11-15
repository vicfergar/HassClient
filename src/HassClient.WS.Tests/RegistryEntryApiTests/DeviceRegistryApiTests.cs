using HassClient.Models;
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
            var devices = await this.hassWSApi.Devices.ListAsync();

            Assert.NotNull(devices);
            Assert.IsNotEmpty(devices);

            Assert.IsTrue(devices.All(d => d.Id != null));
            Assert.IsTrue(devices.Any(d => d.Name != null));
            Assert.IsTrue(devices.All(d => d.OriginalName != null));            
            Assert.IsTrue(devices.All(d => d.ConfigurationEntries != null));
            Assert.IsTrue(devices.All(d => d.ConfigurationEntries.Any()));
            Assert.IsTrue(devices.All(d => d.Connections != null));
            Assert.IsTrue(devices.All(d => d.Identifiers != null));
            Assert.IsTrue(devices.All(d => d.Identifiers.Any()));
            Assert.IsTrue(devices.Any(d => d.EntryType == DeviceEntryTypes.Service));
            Assert.IsTrue(devices.All(d => d.Labels != null));
            Assert.IsTrue(devices.All(d => d.PrimaryConfigEntry != null));
        }

        [Test]
        public async Task UpdateDeviceName()
        {
            var devices = await this.hassWSApi.Devices.ListAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newName = $"TestDevice_{DateTime.Now.Ticks}";
            testDevice.Name = newName;
            var originalModificationDate = testDevice.ModifiedAt;
            var result = await this.hassWSApi.Devices.UpdateAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newName, testDevice.Name);
            Assert.Greater(testDevice.ModifiedAt, originalModificationDate);
        }

        [Test]
        public async Task UpdateDeviceAreaId()
        {
            var devices = await this.hassWSApi.Devices.ListAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newAreaId = $"{DateTime.Now.Ticks}";
            testDevice.AreaId = newAreaId;
            var originalModificationDate = testDevice.ModifiedAt;
            var result = await this.hassWSApi.Devices.UpdateAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.AreEqual(newAreaId, testDevice.AreaId);
            Assert.Greater(testDevice.ModifiedAt, originalModificationDate);
        }

        [Test]
        public async Task UpdateDeviceLabels()
        {
            var devices = await this.hassWSApi.Devices.ListAsync();
            var testDevice = devices.FirstOrDefault();
            Assert.NotNull(testDevice, "SetUp failed");

            var newLabel = $"TestLabel_{DateTime.Now.Ticks}";
            testDevice.Labels.Add(newLabel);
            var originalModificationDate = testDevice.ModifiedAt;
            var result = await this.hassWSApi.Devices.UpdateAsync(testDevice);

            Assert.IsTrue(result);
            Assert.IsFalse(testDevice.HasPendingChanges);
            Assert.IsTrue(testDevice.Labels.Contains(newLabel));
            Assert.Greater(testDevice.ModifiedAt, originalModificationDate);
        }
    }
}
