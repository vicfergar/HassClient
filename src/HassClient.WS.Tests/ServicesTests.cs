using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(ServicesTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(ServicesTests) + "WithRealServer")]
    public class ServicesTests : BaseHassWSApiTest
    {
        public ServicesTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public async Task GetServices()
        {
            var services = await this.hassWSApi.GetServicesAsync();

            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
        }

        [Test]
        public async Task CallService()
        {
            var result = await this.hassWSApi.CallServiceAsync("homeassistant", "check_config");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceForEntities()
        {
            var result = await this.hassWSApi.CallServiceForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

            Assert.NotNull(result);
        }
    }
}
