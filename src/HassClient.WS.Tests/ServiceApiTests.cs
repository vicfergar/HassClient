using HassClient.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class ServiceApiTests : BaseHassWSApiTest
    {
        [Test]
        public async Task GetServices()
        {
            System.Collections.Generic.IEnumerable<ServiceDomain> services = await this.hassWSApi.GetServicesAsync();

            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
            CollectionAssert.AllItemsAreNotNull(services);
        }

        [Test]
        public async Task CallService()
        {
            Context result = await this.hassWSApi.CallServiceAsync("homeassistant", "check_config");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceForEntities()
        {
            bool result = await this.hassWSApi.CallServiceForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

            Assert.NotNull(result);
        }

        [Test]
        public async Task CallServiceWithKnwonDomain()
        {
            bool result = await this.hassWSApi.CallServiceAsync(KnownDomains.Homeassistant, KnownServices.CheckConfig);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CallServiceForEntitiesWithKnwonDomain()
        {
            bool result = await this.hassWSApi.CallServiceForEntitiesAsync(KnownDomains.Homeassistant, KnownServices.UpdateEntity, "sun.sun");

            Assert.IsTrue(result);
        }
    }
}
