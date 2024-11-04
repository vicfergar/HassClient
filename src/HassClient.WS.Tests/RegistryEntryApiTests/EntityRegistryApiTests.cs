using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class EntityRegistryApiTests : BaseHassWSApiTest
    {
        private InputBoolean testInputBoolean;

        private string testEntityId;

        protected override async Task OneTimeSetUp()
        {
            await base.OneTimeSetUp();
            this.testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:switch");
            var result = await this.hassWSApi.CreateStorageEntityRegistryEntryAsync(this.testInputBoolean);
            this.testEntityId = this.testInputBoolean.EntityId;

            Assert.IsTrue(result, "SetUp failed");
        }

        protected override async Task OneTimeTearDown()
        {
            await base.OneTimeTearDown();
            await this.hassWSApi.DeleteStorageEntityRegistryEntryAsync(this.testInputBoolean);
        }

        [Test]
        public async Task GetEntities()
        {
            var entities = await this.hassWSApi.GetEntitiesAsync();

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
            Assert.IsTrue(entities.All(e => e.EntityId != null));
            Assert.IsTrue(entities.All(e => e.Platform != null), entities.FirstOrDefault(e => e.Platform == null)?.EntityId);
            Assert.IsTrue(entities.Any(e => e.ConfigEntryId != null));
            Assert.IsTrue(entities.Any(e => e.HasEntityName));
            Assert.IsTrue(entities.All(e => e.Id != null));
            Assert.IsTrue(entities.Any(e => e.Name != null));
            Assert.IsTrue(entities.Any(e => e.OriginalName != null));
            Assert.IsTrue(entities.All(e => e.Options != null));
            Assert.IsTrue(entities.Any(e => e.Options.Any()));
            Assert.IsTrue(entities.All(e => e.Categories != null));
            Assert.IsTrue(entities.All(e => e.Aliases != null));
            Assert.IsTrue(entities.All(e => e.Labels != null));
        }

        [Test]
        public void GetEntityWithNullEntityIdThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => this.hassWSApi.GetEntityAsync(null));
        }

        [Test]
        public void UpdateEntityWithSameEntityIdThrows()
        {
            var testEntity = new EntityRegistryEntry("switch.TestEntity", null, null);

            Assert.ThrowsAsync<ArgumentException>(() => this.hassWSApi.UpdateEntityAsync(testEntity, testEntity.EntityId));
        }

        [Test]
        public async Task GetEntity()
        {
            var entityId = "light.bed_light";
            var entity = await this.hassWSApi.GetEntityAsync(entityId);

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            Assert.IsNotNull(entity.ConfigEntryId);
            Assert.AreEqual(entityId, entity.EntityId);
        }

        [Test, Order(1), NonParallelizable]
        public async Task GetCreatedEntity()
        {
            var entity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Id);
            Assert.IsNotNull(entity.OriginalName);
            Assert.IsNotNull(entity.OriginalIcon);
            Assert.IsNotNull(entity.Name);
            Assert.IsNotNull(entity.Icon);
            Assert.AreEqual(this.testEntityId, entity.EntityId);
        }

        [Order(1), NonParallelizable]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdateEntityDisable(bool disable)
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            var result = await this.hassWSApi.UpdateEntityAsync(testEntity, disable: disable);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.AreEqual(disable, testEntity.IsDisabled);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityName()
        {
            var newName = MockHelpers.GetRandomTestName();
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            testEntity.Name = newName;
            var originalModificationDate = testEntity.ModifiedAt;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.AreEqual(newName, testEntity.Name);
            Assert.AreNotEqual(newName, testEntity.OriginalName);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityIcon()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            var newIcon = "mdi:fan";
            testEntity.Icon = newIcon;
            var originalModificationDate = testEntity.ModifiedAt;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.AreEqual(newIcon, testEntity.Icon);
            Assert.AreNotEqual(newIcon, testEntity.OriginalIcon);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityAliases()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
                
            testEntity.Aliases.Add("alias3");
            var originalModificationDate = testEntity.ModifiedAt;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.Contains("alias3", testEntity.Aliases as ICollection);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityLabels()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
                
            testEntity.Labels.Add("label3");
            var originalModificationDate = testEntity.ModifiedAt;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.Contains("label3", testEntity.Labels as ICollection);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);
        }

        [Test, Order(1)]
        public async Task RefreshEntity()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var clonedEntity = testEntity.Clone();
            clonedEntity.Name = MockHelpers.GetRandomTestName();
            var result = await this.hassWSApi.UpdateEntityAsync(clonedEntity);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(testEntity.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.RefreshEntityAsync(testEntity);
            Assert.IsTrue(result);
            Assert.AreEqual(clonedEntity.Name, testEntity.Name);
        }

        [Test, Order(2), NonParallelizable]
        public async Task UpdateEntityId()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var newEntityId = this.testEntityId + 1;
            
            var originalModificationDate = testEntity.ModifiedAt;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity, newEntityId);

            Assert.IsTrue(result);
            Assert.AreEqual(newEntityId, testEntity.EntityId);
            Assert.AreNotEqual(this.testEntityId, newEntityId);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);

            this.testEntityId = newEntityId; // This is needed for DeleteEntityTest
        }

        [Test, Order(3)]
        public async Task UpdateWithForce()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var initialName = testEntity.Name;
            var initialIcon = testEntity.Icon;
            var initialDisabledBy = testEntity.DisabledBy;
            var clonedEntry = testEntity.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            var result = await this.hassWSApi.UpdateEntityAsync(clonedEntry, disable: true);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(testEntity.HasPendingChanges, "SetUp failed");

            var originalModificationDate = testEntity.ModifiedAt;
            result = await this.hassWSApi.UpdateEntityAsync(testEntity, disable: false, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, testEntity.Name);
            Assert.AreEqual(initialIcon, testEntity.Icon);
            Assert.AreEqual(initialDisabledBy, testEntity.DisabledBy);
            Assert.Greater(testEntity.ModifiedAt, originalModificationDate);
        }

        [Test, Order(4), NonParallelizable]
        public async Task DeleteEntity()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var result = await this.hassWSApi.DeleteEntityAsync(testEntity);
            var testEntity1 = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            Assert.IsTrue(result);
            Assert.IsNull(testEntity1);
            Assert.IsFalse(testEntity.IsTracked);
        }
    }
}
