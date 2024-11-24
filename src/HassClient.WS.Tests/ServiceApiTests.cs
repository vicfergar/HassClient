using HassClient.Models;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class ServiceApiTests : BaseHassWSApiTest
    {
        [Test]
        public async Task GetServices()
        {
            var services = await this.hassWSApi.Services.ListAsync();

            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
            Assert.IsTrue(services.All(x => !string.IsNullOrEmpty(x.Domain)));
        }

        [Test]
        public async Task CallService()
        {
            var result = await this.hassWSApi.Services.CallAsync("homeassistant", "check_config");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceForEntities()
        {
            var result = await this.hassWSApi.Services.CallForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceWithKnownDomain()
        {
            var result = await this.hassWSApi.Services.CallAsync(KnownDomains.Homeassistant, KnownServices.CheckConfig);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CallServiceForEntitiesWithKnownDomain()
        {
            var result = await this.hassWSApi.Services.CallForEntitiesAsync(KnownDomains.Homeassistant, KnownServices.UpdateEntity, "sun.sun");

            Assert.IsTrue(result);
        }
    }
}
