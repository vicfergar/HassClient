using HassClient.WS.Messages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class SearchApiTests : BaseHassWSApiTest
    {
        public static IEnumerable<ItemTypes> GetAllItemTypes()
        {
            return Enum.GetValues(typeof(ItemTypes)).Cast<ItemTypes>();
        }

        public static IEnumerable<ItemTypes> GetUnsearchableItemTypes()
        {
            return new[] { ItemTypes.Integration };
        }

        public static IEnumerable<ItemTypes> GetSearchableItemTypes()
        {
            return GetAllItemTypes().Except(GetUnsearchableItemTypes());
        }

        [TestCaseSource(nameof(GetAllItemTypes))]
        public void AllItemTypesHaveEquivalentProperties(ItemTypes itemType)
        {
            var responseProperties = typeof(SearchRelatedResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var propertyName = $"{itemType}Ids";
            var hasProperty = responseProperties.Any(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            Assert.IsTrue(hasProperty, $"Property '{propertyName}' is missing for ItemType '{itemType}'.");
        }

        [TestCaseSource(nameof(GetSearchableItemTypes))]
        public async Task SearchRelatedAsync_NonexistentItemId_ReturnsNull(ItemTypes itemType)
        {
            var result = await this.hassWSApi.SearchRelatedAsync(itemType, $"light.Nonexisting_{DateTime.Now.Ticks}");

            Assert.NotNull(result);
            Assert.IsEmpty(result.AreaIds);
            Assert.IsEmpty(result.AutomationBlueprintIds);
            Assert.IsEmpty(result.AutomationIds);
            Assert.IsEmpty(result.ConfigEntryIds);
            Assert.IsEmpty(result.DeviceIds);
            Assert.IsEmpty(result.EntityIds);
            Assert.IsEmpty(result.FloorIds);
            Assert.IsEmpty(result.GroupIds);
            Assert.IsEmpty(result.HelperIds);
            Assert.IsEmpty(result.IntegrationIds);
            Assert.IsEmpty(result.LabelIds);
            Assert.IsEmpty(result.PersonIds);
            Assert.IsEmpty(result.SceneIds);
            Assert.IsEmpty(result.ScriptBlueprintIds);
            Assert.IsEmpty(result.ScriptIds);
        }

        [TestCaseSource(nameof(GetUnsearchableItemTypes))]
        public void SearchRelatedAsync_UnsearchableItemType_ThrowsException(ItemTypes itemType)
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await this.hassWSApi.SearchRelatedAsync(itemType, $"Unknown_{DateTime.Now.Ticks}");
            });

            Assert.That(ex.Message, Is.EqualTo("Unknown error occurred: Unknown error"));
        }

        [Test]
        public async Task SearchRelatedExistentEntity()
        {
            var result = await this.hassWSApi.SearchRelatedAsync(ItemTypes.Entity, "light.bed_light");

            Assert.NotNull(result);
            Assert.IsEmpty(result.AreaIds);
            Assert.IsEmpty(result.AutomationBlueprintIds);
            Assert.IsEmpty(result.AutomationIds);
            Assert.IsNotEmpty(result.ConfigEntryIds);
            Assert.IsNotEmpty(result.DeviceIds);
            Assert.IsEmpty(result.EntityIds);
            Assert.IsEmpty(result.FloorIds);
            Assert.IsEmpty(result.GroupIds);
            Assert.IsEmpty(result.HelperIds);
            Assert.IsNotEmpty(result.IntegrationIds);
            Assert.AreEqual("demo", result.IntegrationIds[0]);
            Assert.IsEmpty(result.LabelIds);
            Assert.IsEmpty(result.PersonIds);
            Assert.IsEmpty(result.SceneIds);
            Assert.IsEmpty(result.ScriptBlueprintIds);
            Assert.IsEmpty(result.ScriptIds);
        }
    }
}
