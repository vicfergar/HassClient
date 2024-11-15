using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class EntitySourcesApiTests : BaseHassWSApiTest
    {
        [Test]
        public async Task GetEntitySources()
        {
            var entities = await this.hassWSApi.ListEntitySourcesAsync();

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
        }
    }
}
