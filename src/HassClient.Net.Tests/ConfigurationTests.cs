using HassClient.Net.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
{
    [TestFixture(true, TestName = nameof(ConfigurationTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(ConfigurationTests) + "WithRealServer")]
    public class ConfigurationTests : BaseHassWSApiTest
    {

        private Configuration configuration;

        public ConfigurationTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test]
        public async Task GetConfiguration()
        {
            if (this.configuration != null)
            {
                return;
            }

            this.configuration = await this.hassWSApi.GetConfigurationAsync();

            Assert.IsNotNull(this.configuration);
        }

        [Test]
        public void ConfigurationHasAllowedExternalDirs()
        {
            Assert.NotNull(this.configuration.AllowedExternalDirs);
            Assert.IsNotEmpty(this.configuration.AllowedExternalDirs);
        }

        [Test]
        public void ConfigurationHasAllowedExternalUrls()
        {
            Assert.NotNull(this.configuration.AllowedExternalUrls);
        }

        [Test]
        public void ConfigurationHasComponents()
        {
            Assert.NotNull(this.configuration.Components);
            Assert.IsNotEmpty(this.configuration.Components);
        }

        [Test]
        public void ConfigurationHasConfigDirectory()
        {
            Assert.NotNull(this.configuration.ConfigDirectory);
        }

        [Test]
        public void ConfigurationHasConfigSource()
        {
            Assert.NotNull(this.configuration.ConfigSource);
        }

        [Test]
        public void ConfigurationHasLocation()
        {
            Assert.NotNull(this.configuration.LocationName);
            Assert.NotZero(this.configuration.Latitude);
            Assert.NotZero(this.configuration.Longitude);
        }

        [Test]
        public void ConfigurationHasState()
        {
            Assert.NotNull(this.configuration.State);
        }

        [Test]
        public void ConfigurationHasTimeZone()
        {
            Assert.NotNull(this.configuration.TimeZone);
        }

        [Test]
        public void ConfigurationHasUnitSystem()
        {
            Assert.NotNull(this.configuration.UnitSystem);
            Assert.NotNull(this.configuration.UnitSystem.Length);
            Assert.NotNull(this.configuration.UnitSystem.Mass);
            Assert.NotNull(this.configuration.UnitSystem.Pressure);
            Assert.NotNull(this.configuration.UnitSystem.Temperature);
            Assert.NotNull(this.configuration.UnitSystem.Volume);
        }

        [Test]
        public void ConfigurationHasVersion()
        {
            Assert.NotNull(this.configuration.Version);
        }
    }
}
