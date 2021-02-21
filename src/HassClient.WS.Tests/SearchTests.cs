using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(SearchTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(SearchTests) + "WithRealServer")]
    public class SearchTests : BaseHassWSApiTest
    {
        public SearchTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        public static Array GetItemTypes()
        {
            return Enum.GetValues(typeof(ItemTypes));
        }

        [TestCaseSource(nameof(GetItemTypes))]
        public async Task SearchRelatedUnknown(ItemTypes itemTypes)
        {
            var result = await this.hassWSApi.SearchRelated(itemTypes, $"Unknown_{DateTime.Now.Ticks}");

            Assert.NotNull(result);
            Assert.IsNull(result.AreaIds);
            Assert.IsNull(result.AutomationIds);
            Assert.IsNull(result.ConfigEntryIds);
            Assert.IsNull(result.DeviceIds);
            Assert.IsNull(result.EntityIds);
        }

        [Test]
        public async Task SearchRelatedKnownEntity()
        {
            var result = await this.hassWSApi.SearchRelated(ItemTypes.Entity, "weather.home");

            Assert.NotNull(result);
            Assert.NotNull(result.ConfigEntryIds);
            Assert.NotNull(result.DeviceIds);
            Assert.NotNull(result.EntityIds);
            Assert.NotZero(result.ConfigEntryIds.Length);
            Assert.NotZero(result.DeviceIds.Length);
            Assert.NotZero(result.EntityIds.Length);
        }
    }
}
