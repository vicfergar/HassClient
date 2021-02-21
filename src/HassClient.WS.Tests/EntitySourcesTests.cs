using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(EntitySourcesTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(EntitySourcesTests) + "WithRealServer")]
    public class EntitySourcesTests : BaseHassWSApiTest
    {
        public EntitySourcesTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public async Task GetEntitySources()
        {
            var entities = await this.hassWSApi.GetEntitySourcesAsync();

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
        }

        [Test]
        public async Task GetEntitySourceWithFilterAsync()
        {
            var entityId = "zone.home";
            var result = await this.hassWSApi.GetEntitySourceAsync(entityId);

            Assert.AreEqual(result.EntityId, entityId);
            Assert.AreEqual(result.Domain, entityId.Split('.')[0]);
            Assert.NotNull(result.Source);
        }
    }
}
