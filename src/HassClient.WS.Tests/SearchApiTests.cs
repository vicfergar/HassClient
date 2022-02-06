using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class SearchApiTests : BaseHassWSApiTest
    {
        public static Array GetItemTypes()
        {
            return Enum.GetValues(typeof(ItemTypes));
        }

        [TestCaseSource(nameof(GetItemTypes))]
        public async Task SearchRelatedUnknown(ItemTypes itemTypes)
        {
            var result = await this.hassWSApi.SearchRelatedAsync(itemTypes, $"Unknown_{DateTime.Now.Ticks}");

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
            var result = await this.hassWSApi.SearchRelatedAsync(ItemTypes.Entity, "light.bed_light");

            Assert.NotNull(result);
            Assert.NotNull(result.ConfigEntryIds);
            Assert.NotNull(result.DeviceIds);
            Assert.NotZero(result.ConfigEntryIds.Length);
            Assert.NotZero(result.DeviceIds.Length);
        }
    }
}
