using HassClient.Models;
using HassClient.Serialization;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(AreaRegistryTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(AreaRegistryTests) + "WithRealServer")]
    public class AreaRegistryTests : BaseHassWSApiTest
    {
        private Area testArea;
        
        public AreaRegistryTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public void NewAreaHasNoPendingChanges()
        {
            var testArea = HassSerializer.DeserializeObject<Area>("{}");
            Assert.IsFalse(testArea.HasPendingChanges);
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var initialName = $"TestArea_{DateTime.Now.Ticks}";
            var testArea = new Area(initialName);

            testArea.Name = $"TestArea_{DateTime.Now.Ticks}";
            Assert.IsTrue(testArea.HasPendingChanges);

            testArea.Name = initialName;
            Assert.False(testArea.HasPendingChanges);
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateArea()
        {
            if (this.testArea == null)
            {
                this.testArea = await this.hassWSApi.CreateAreaAsync($"TestArea_{DateTime.Now.Ticks}");
                Assert.NotNull(this.testArea, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testArea.Id);
            Assert.NotNull(this.testArea.Name);
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
            this.testArea.Name = $"TestArea_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateAreaAsync(this.testArea);

            Assert.IsTrue(result);
            Assert.False(this.testArea.HasPendingChanges);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteArea()
        {
            if (this.testArea == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteAreaAsync(this.testArea);
            this.testArea = null;

            Assert.IsTrue(result);
        }
    }
}
