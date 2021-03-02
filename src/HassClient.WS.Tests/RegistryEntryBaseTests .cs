using HassClient.Models;
using HassClient.Serialization;
using NUnit.Framework;
using System;

namespace HassClient.WS.Tests
{
    [TestFixture(TestOf = typeof(RegistryEntryBase))]
    public class RegistryEntryBaseTests
    {
        private class TestRegistryEntry : RegistryEntryBase
        {
            public override string EntityId => null;

            public override string UniqueId { get; internal set; }

            public TestRegistryEntry() : base(null, null) { }
        }

        [Test]
        public void NewRegistryEntryHasNoPendingChanges()
        {
            var testRegistryEntry = HassSerializer.DeserializeObject<TestRegistryEntry>("{}");
            Assert.IsFalse(testRegistryEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testRegistryEntry = new TestRegistryEntry();

            testRegistryEntry.Name = $"{nameof(RegistryEntryBaseTests)}_{DateTime.Now.Ticks}";
            Assert.IsTrue(testRegistryEntry.HasPendingChanges);

            testRegistryEntry.Name = null;
            Assert.False(testRegistryEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testRegistryEntry = new TestRegistryEntry();

            testRegistryEntry.Icon = "icon";
            Assert.IsTrue(testRegistryEntry.HasPendingChanges);

            testRegistryEntry.Icon = null;
            Assert.False(testRegistryEntry.HasPendingChanges);
        }
    }
}
