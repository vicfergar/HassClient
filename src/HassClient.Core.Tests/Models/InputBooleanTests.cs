using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(InputBoolean))]
    public class InputBooleanTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(InputBoolean).GetConstructors()
                                                  .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewInputBooleanHasPendingChanges()
        {
            var testEntry = new InputBoolean(MockHelpers.GetRandomTestName());
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewInputBooleanIsUntracked()
        {
            var testEntry = new InputBoolean(MockHelpers.GetRandomTestName());
            Assert.False(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewInputBooleanWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new InputBoolean(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialName, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialIcon, out _);

            testEntry.Icon = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewInitialMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initial);

            testEntry.Initial = !initial;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Initial = initial;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialName, out var initialIcon, out var initial);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = MockHelpers.GetRandomTestName();
            testEntry.Initial = !initial;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initial, testEntry.Initial);
        }

        private InputBoolean CreateTestEntry(out string entityId, out string name, out string icon, out bool initial)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.InputBoolean);
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:fan";
            initial = true;
            return InputBoolean.CreateUnmodified(entityId, name, icon, initial);
        }
    }
}
