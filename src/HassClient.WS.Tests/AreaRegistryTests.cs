using HassClient.Models;
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
