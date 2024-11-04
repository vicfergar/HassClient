using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class FloorRegistryApiTests : BaseHassWSApiTest
    {
        private Floor testFloor;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateFloor()
        {
            if (this.testFloor == null)
            {
                this.testFloor = new Floor(MockHelpers.GetRandomTestName(), "mdi:lamp", 1, new[] { "alias1", "alias2" });
                var result = await this.hassWSApi.CreateFloorAsync(this.testFloor);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testFloor.Id);
            Assert.NotNull(this.testFloor.Name);
            Assert.NotNull(this.testFloor.Icon);
            Assert.NotNull(this.testFloor.Level);
            Assert.IsFalse(this.testFloor.HasPendingChanges);
            Assert.IsTrue(this.testFloor.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetFloors()
        {
            var floors = await this.hassWSApi.GetFloorsAsync();

            Assert.NotNull(floors);
            Assert.IsNotEmpty(floors);
            Assert.IsTrue(floors.Contains(this.testFloor));
        }

        [Test, Order(3)]
        public async Task UpdateFloor()
        {
            this.testFloor.Name = MockHelpers.GetRandomTestName();
            this.testFloor.Icon = "mdi:sofa";
            this.testFloor.Level = 2;
            this.testFloor.Aliases.Add("alias3");
            var originalModificationDate = this.testFloor.ModifiedAt;
            var result = await this.hassWSApi.UpdateFloorAsync(this.testFloor);

            Assert.IsTrue(result);
            Assert.False(this.testFloor.HasPendingChanges);
            Assert.Greater(this.testFloor.ModifiedAt, originalModificationDate);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteFloor()
        {
            if (this.testFloor == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteFloorAsync(this.testFloor);
            var deletedFloor = this.testFloor;
            this.testFloor = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedFloor.IsTracked);
        }
    }
}
