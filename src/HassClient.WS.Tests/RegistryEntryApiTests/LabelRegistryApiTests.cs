using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class LabelRegistryApiTests : BaseHassWSApiTest
    {
        private Label testLabel;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateLabel()
        {
            if (this.testLabel == null)
            {
                this.testLabel = new Label(MockHelpers.GetRandomTestName(), "mdi:lamp", "#000000", "Test description");
                var result = await this.hassWSApi.CreateLabelAsync(this.testLabel);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testLabel.Id);
            Assert.NotNull(this.testLabel.Name);
            Assert.NotNull(this.testLabel.Color);
            Assert.NotNull(this.testLabel.Description);
            Assert.IsFalse(this.testLabel.HasPendingChanges);
            Assert.IsTrue(this.testLabel.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetLabels()
        {
            var labels = await this.hassWSApi.GetLabelsAsync();

            Assert.NotNull(labels);
            Assert.IsNotEmpty(labels);
            Assert.IsTrue(labels.Contains(this.testLabel));
        }

        [Test, Order(3)]
        public async Task UpdateLabel()
        {
            this.testLabel.Name = MockHelpers.GetRandomTestName();
            this.testLabel.Color = "#FFFFFF";
            this.testLabel.Description = "Updated description"; 
            var originalModificationDate = this.testLabel.ModifiedAt;
            var result = await this.hassWSApi.UpdateLabelAsync(this.testLabel);

            Assert.IsTrue(result);
            Assert.False(this.testLabel.HasPendingChanges);
            Assert.Greater(this.testLabel.ModifiedAt, originalModificationDate);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteLabel()
        {
            if (this.testLabel == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteLabelAsync(this.testLabel);
            var deletedLabel = this.testLabel;
            this.testLabel = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedLabel.IsTracked);
        }
    }
}
