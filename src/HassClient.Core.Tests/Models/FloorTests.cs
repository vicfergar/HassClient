using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Floor))]
    public class FloorTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Floor).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewAreaHasPendingChanges()
        {
            var testEntry = new Floor(MockHelpers.GetRandomTestName());
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewAreaIsUntracked()
        {
            var testEntry = new Floor(MockHelpers.GetRandomTestName());
            Assert.False(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => NamedEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Floor(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out _, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialIcon, out _, out _);

            testEntry.Icon = "mdi:lamp";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewLevelMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialLevel, out _);

            testEntry.Level = 2;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Level = initialLevel;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]  
        public void SetNewAliasesMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _);

            testEntry.Aliases.Add("alias3");
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Aliases.Remove("alias3");
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out var initialIcon, out var initialLevel, out var initialAliases);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = "mdi:lamp";
            testEntry.Level = 2;
            testEntry.Aliases.Add("alias3");
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initialLevel, testEntry.Level);
            CollectionAssert.AreEqual(initialAliases, testEntry.Aliases);
        }

        private Floor CreateTestEntry(out string name, out string icon, out int level, out string[] aliases)
        {
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:floor";
            level = 1;
            aliases = new[] { "alias1", "alias2" };  
            return Floor.CreateUnmodified(name, icon, level, aliases);
        }
    }
}
