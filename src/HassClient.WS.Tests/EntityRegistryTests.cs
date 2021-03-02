using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(EntityRegistryTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(EntityRegistryTests) + "WithRealServer")]
    public class EntityRegistryTests : BaseHassWSApiTest
    {
        private InputBoolean testInputBoolean;

        private string testEntityId;

        public EntityRegistryTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        protected override async Task OneTimeSetUp()
        {
            await base.OneTimeSetUp();
            this.testInputBoolean = new InputBoolean($"{nameof(EntityRegistryTests)}_{DateTime.Now.Ticks}");
            var result = await this.hassWSApi.CreateInputBooleanAsync(this.testInputBoolean);
            this.testEntityId = this.testInputBoolean.EntityId;

            Assert.IsTrue(result, "SetUp failed");
        }

        protected override async Task OneTimeTearDown()
        {
            await base.OneTimeTearDown();
            await this.hassWSApi.DeleteInputBooleanAsync(this.testInputBoolean);
        }

        [Test]
        public async Task GetEntities()
        {
            var entities = await this.hassWSApi.GetEntitiesAsync();

            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
            Assert.IsTrue(entities.All(e => e.EntityId != null));
            Assert.IsTrue(entities.All(e => e.Platform != null));
            Assert.IsTrue(entities.Any(e => e.ConfigEntryId != null));
            Assert.IsTrue(entities.Any(e => e.DisabledBy != DisabledByEnum.None));
        }

        [Test]
        public async Task GetEntity()
        {
            var entityId = "weather.home";
            var entity = await this.hassWSApi.GetEntityAsync(entityId);

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.ConfigEntryId);
            Assert.AreEqual(entityId, entity.EntityId);
        }

        [Test]
        public void GetEntityWithNullEntityIdThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => this.hassWSApi.GetEntityAsync(null));
        }

        [Test]
        public void UpdateEntityWithSameEntityIdThrows()
        {
            var testEntity = new RegistryEntry("switch.TestEntity", null, null);

            Assert.ThrowsAsync<ArgumentException>(() => this.hassWSApi.UpdateEntityAsync(testEntity, testEntity.EntityId));
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
            var newName = $"Test_{DateTime.Now.Ticks}";
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            testEntity.Name = newName;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.AreEqual(newName, testEntity.Name);
            Assert.AreNotEqual(newName, testEntity.OriginalName);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityIcon()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            var newIcon = "mdi:fan";
            testEntity.Icon = newIcon;
            var result = await this.hassWSApi.UpdateEntityAsync(testEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(this.testEntityId, testEntity.EntityId);
            Assert.AreEqual(newIcon, testEntity.Icon);
            Assert.AreNotEqual(newIcon, testEntity.OriginalIcon);
        }

        [Test, Order(1), NonParallelizable]
        public async Task UpdateEntityEntityId()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var newEntityId = this.testEntityId + 1;

            var result = await this.hassWSApi.UpdateEntityAsync(testEntity, newEntityId);

            Assert.IsTrue(result);
            Assert.AreEqual(newEntityId, testEntity.EntityId);
            Assert.AreNotEqual(this.testEntityId, newEntityId);

            this.testEntityId = newEntityId; // This is needed to DeleteEntityTest
        }

        [Test, Order(2), NonParallelizable]
        public async Task DeleteEntity()
        {
            var testEntity = await this.hassWSApi.GetEntityAsync(this.testEntityId);
            var result = await this.hassWSApi.DeleteEntityAsync(testEntity);
            var testEntity1 = await this.hassWSApi.GetEntityAsync(this.testEntityId);

            Assert.IsTrue(result);
            Assert.IsNull(testEntity1);
        }
    }
}
