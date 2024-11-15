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
                this.testArea = new Area(MockHelpers.GetRandomTestName(), icon: "mdi:home", aliases: new[] { "alias1", "alias2" }, labels: new[] { "label1", "label2" });
                var result = await this.hassWSApi.Areas.CreateAsync(testArea);
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
            var areas = await this.hassWSApi.Areas.ListAsync();

            Assert.NotNull(areas);
            Assert.IsNotEmpty(areas);
            Assert.IsTrue(areas.Contains(this.testArea));
            Assert.AreEqual(this.testArea.Name, areas.First().Name);
            Assert.AreEqual(this.testArea.Icon, areas.First().Icon);
            Assert.That(areas.First().Aliases, Is.EquivalentTo(this.testArea.Aliases));
            Assert.That(areas.First().Labels, Is.EquivalentTo(this.testArea.Labels));
        }

        [Test, Order(3)]
        public async Task UpdateArea()
        {
            this.testArea.Name = MockHelpers.GetRandomTestName();
            this.testArea.Picture = "https://www.home-assistant.io/images/favicon-192x192.png";
            this.testArea.Icon = "mdi:home";
            this.testArea.FloorId = "ground_floor";
            this.testArea.Aliases.Add("alias3");
            this.testArea.Labels.Add("label3");
            var originalModificationDate = this.testArea.ModifiedAt;
            var result = await this.hassWSApi.Areas.UpdateAsync(this.testArea);

            Assert.IsTrue(result);
            Assert.False(this.testArea.HasPendingChanges);
            Assert.Greater(this.testArea.ModifiedAt, originalModificationDate);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteArea()
        {
            if (this.testArea == null)
            {
                return;
            }

            var result = await this.hassWSApi.Areas.DeleteAsync(this.testArea);
            var deletedArea = this.testArea;
            this.testArea = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedArea.IsTracked);
        }
    }
}
