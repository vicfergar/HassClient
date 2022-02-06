using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class ZoneApiTests : BaseHassWSApiTest
    {
        private Zone testZone;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateZone()
        {
            if (this.testZone == null)
            {
                this.testZone = new Zone(MockHelpers.GetRandomTestName(), 20.1f, 34.6f, 10.5f, "mdi:fan", true);
                var result = await this.hassWSApi.CreateStorageEntityRegistryEntryAsync(this.testZone);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testZone);
            Assert.NotNull(this.testZone.UniqueId);
            Assert.NotNull(this.testZone.Name);
            Assert.IsFalse(this.testZone.HasPendingChanges);
            Assert.IsTrue(this.testZone.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetZones()
        {
            var result = await this.hassWSApi.GetStorageEntityRegistryEntriesAsync<Zone>();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains(this.testZone));
            Assert.IsTrue(result.All(x => x.Id != null));
            Assert.IsTrue(result.All(x => x.UniqueId != null));
            Assert.IsTrue(result.All(x => x.EntityId.StartsWith("zone.")));
            Assert.IsTrue(result.Any(x => x.Name != null));
            Assert.IsTrue(result.Any(x => x.Longitude > 0));
            Assert.IsTrue(result.Any(x => x.Latitude > 0));
            Assert.IsTrue(result.Any(x => x.Longitude != x.Latitude));
            Assert.IsTrue(result.Any(x => x.Radius > 0));
            Assert.IsTrue(result.Any(x => x.IsPassive == true));
            Assert.IsTrue(result.Any(x => x.Icon != null));
        }

        [Test, Order(3)]
        public async Task UpdateZoneName()
        {
            this.testZone.Name = $"{nameof(ZoneApiTests)}_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testZone);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testZone.HasPendingChanges);
        }

        [Test, Order(4)]
        public async Task UpdateZoneInitial()
        {
            this.testZone.IsPassive = false;
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testZone);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testZone.HasPendingChanges);
        }

        [Test, Order(5)]
        public async Task UpdateZoneIcon()
        {
            this.testZone.Icon = $"mdi:lightbulb";
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testZone);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testZone.HasPendingChanges);
        }

        [Test, Order(6)]
        public async Task UpdateWithForce()
        {
            var initialName = this.testZone.Name;
            var initialIcon = this.testZone.Icon;
            var initialLongitude = this.testZone.Longitude;
            var initialLatitude = this.testZone.Latitude;
            var initialRadius = this.testZone.Radius;
            var initialIsPassive = this.testZone.IsPassive;
            var clonedEntry = this.testZone.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            clonedEntry.Longitude = initialLongitude + 15f;
            clonedEntry.Latitude = initialLatitude + 15f;
            clonedEntry.Radius = initialRadius + 15f;
            clonedEntry.IsPassive = !initialIsPassive;
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(this.testZone.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testZone, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, this.testZone.Name);
            Assert.AreEqual(initialIcon, this.testZone.Icon);
            Assert.AreEqual(initialLongitude, this.testZone.Longitude);
            Assert.AreEqual(initialLatitude, this.testZone.Latitude);
            Assert.AreEqual(initialRadius, this.testZone.Radius);
            Assert.AreEqual(initialIsPassive, this.testZone.IsPassive);
            Assert.IsFalse(this.testZone.HasPendingChanges);
        }

        [OneTimeTearDown]
        [Test, Order(7)]
        public async Task DeleteZone()
        {
            if (this.testZone == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteStorageEntityRegistryEntryAsync(this.testZone);
            var deletedZone = this.testZone;
            this.testZone = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedZone.IsTracked);
        }
    }
}
