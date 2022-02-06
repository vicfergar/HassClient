using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class AreaRegistryApiTests : BaseHassWSApiTest
    {
        private Area testArea;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateArea()
        {
            if (this.testArea == null)
            {
                this.testArea = new Area(MockHelpers.GetRandomTestName());
                var result = await this.hassWSApi.CreateAreaAsync(testArea);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testArea.Id);
            Assert.NotNull(this.testArea.Name);
            Assert.IsFalse(this.testArea.HasPendingChanges);
            Assert.IsTrue(this.testArea.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetAreas()
        {
            var areas = await this.hassWSApi.GetAreasAsync();

            Assert.NotNull(areas);
            Assert.IsNotEmpty(areas);
            Assert.IsTrue(areas.Contains(this.testArea));
        }

        [Test, Order(3)]
        public async Task UpdateArea()
        {
            this.testArea.Name = MockHelpers.GetRandomTestName();
            var result = await this.hassWSApi.UpdateAreaAsync(this.testArea);

            Assert.IsTrue(result);
            Assert.False(this.testArea.HasPendingChanges);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = this.testArea.Name;
            var clonedArea = this.testArea.Clone();
            clonedArea.Name = $"{initialName}_cloned";
            var result = await this.hassWSApi.UpdateAreaAsync(clonedArea);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(this.testArea.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.UpdateAreaAsync(this.testArea, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, this.testArea.Name);
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeleteArea()
        {
            if (this.testArea == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteAreaAsync(this.testArea);
            var deletedArea = this.testArea;
            this.testArea = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedArea.IsTracked);
        }
    }
}
