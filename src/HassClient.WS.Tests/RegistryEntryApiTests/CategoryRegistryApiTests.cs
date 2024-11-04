using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class CategoryRegistryApiTests : BaseHassWSApiTest
    {
        private Category testCategory;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateCategory()
        {
            if (this.testCategory == null)
            {
                this.testCategory = new Category(MockHelpers.GetRandomTestName(), icon: "mdi:lamp", scope:  "test_scope");
                var result = await this.hassWSApi.CreateCategoryAsync(this.testCategory);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testCategory.Id);
            Assert.NotNull(this.testCategory.Name);
            Assert.NotNull(this.testCategory.Icon);
            Assert.IsFalse(this.testCategory.HasPendingChanges);
            Assert.IsTrue(this.testCategory.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetCategories()
        {
            var categories = await this.hassWSApi.GetCategoriesAsync("test_scope");

            Assert.NotNull(categories);
            Assert.IsNotEmpty(categories);
            Assert.IsTrue(categories.Contains(this.testCategory));
        }

        [Test, Order(3)]
        public async Task UpdateCategory()
        {
            this.testCategory.Name = MockHelpers.GetRandomTestName();
            this.testCategory.Icon = "mdi:sofa";
            var originalModificationDate = this.testCategory.ModifiedAt;
            var result = await this.hassWSApi.UpdateCategoryAsync(this.testCategory);

            Assert.IsTrue(result);
            Assert.False(this.testCategory.HasPendingChanges);
            Assert.Greater(this.testCategory.ModifiedAt, originalModificationDate);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteCategory()
        {
            if (this.testCategory == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteCategoryAsync(this.testCategory);
            var deletedCategory = this.testCategory;
            this.testCategory = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedCategory.IsTracked);
        }
    }
}
