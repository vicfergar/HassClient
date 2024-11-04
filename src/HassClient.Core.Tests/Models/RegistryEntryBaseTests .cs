using HassClient.Models;
using HassClient.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(NamedEntryBase))]
    public class NamedEntryBaseTests
    {
        private class TestRegistryEntry : NamedEntryBase, IEntityEntry
        {
            public string EntityId => MockHelpers.GetRandomEntityId(KnownDomains.Camera);

            internal protected override string UniqueId { get; set; }

            public TestRegistryEntry(string name, string icon = null)
                : base(name, icon)
            {
            }

            public TestRegistryEntry()
                : this(MockHelpers.GetRandomTestName(), "mdi:camera")
            {
            }

            public static TestRegistryEntry CreateUnmodified(out string initialName, out string initialIcon)
            {
                var result = new TestRegistryEntry();
                initialName = result.Name;
                initialIcon = result.Icon;
                result.SaveChanges();
                return result;
            }
        }

        private class NullNameTestRegistryEntry : TestRegistryEntry
        {
            protected override bool AcceptsNullOrWhiteSpaceName => true;

            public NullNameTestRegistryEntry(string name, string icon = null)
                : base(name, icon)
            {
            }
        }

        [Test]
        public void DeserializedEntityRegistryEntryHasNoPendingChanges()
        {
            var testEntry = HassSerializer.DeserializeObject<TestRegistryEntry>("{}");
            Assert.IsFalse(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewEntityRegistryEntryHasPendingChanges()
        {
            var testEntry = new TestRegistryEntry();
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewEntityRegistryEntryIsUntracked()
        {
            var testEntry = new TestRegistryEntry();
            Assert.False(testEntry.IsTracked);
        }

        public static IEnumerable<string> NullOrWhiteSpaceStringValues()
        {
            yield return null;
            yield return string.Empty;
            yield return " ";
        }

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewEntityRegistryEntryWithNullOrWhiteSpaceNameWhenNotAcceptedThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new TestRegistryEntry(value));
        }

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewEntityRegistryEntryWithNullOrWhiteSpaceNameWhenAcceptedDoesNotThrows(string value)
        {
            Assert.DoesNotThrow(() => new NullNameTestRegistryEntry(value));
        }

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void SetNullOrWhiteSpaceNameWhenNotAcceptedThrows(string value)
        {
            var testEntry = new TestRegistryEntry();
            Assert.Throws<InvalidOperationException>(() => testEntry.Name = value);
        }

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void SetNullOrWhiteSpaceNameWhenAcceptedDoesNotThrows(string value)
        {
            var testEntry = new NullNameTestRegistryEntry(MockHelpers.GetRandomTestName());
            Assert.DoesNotThrow(() => testEntry.Name = value);
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = TestRegistryEntry.CreateUnmodified(out var initialName, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = TestRegistryEntry.CreateUnmodified(out _, out var initialIcon);

            testEntry.Icon = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = TestRegistryEntry.CreateUnmodified(out var initialName, out var initialIcon);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
        }
    }
}
