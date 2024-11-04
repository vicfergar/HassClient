using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Label))]
    public class LabelTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Label).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewAreaHasPendingChanges()
        {
            var testEntry = new Label(MockHelpers.GetRandomTestName());
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewAreaIsUntracked()
        {
            var testEntry = new Label(MockHelpers.GetRandomTestName());
            Assert.False(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => NamedEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Label(value));
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
        public void SetNewColorMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialColor, out _);

            testEntry.Color = "blue";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Color = initialColor;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewDescriptionMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initialDescription);

            testEntry.Description = "updated description";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Description = initialDescription;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out var initialIcon, out var initialColor, out var initialDescription);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = "mdi:lamp";
            testEntry.Color = "blue";
            testEntry.Description = "updated description";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initialColor, testEntry.Color);
            Assert.AreEqual(initialDescription, testEntry.Description);
        }

        private Label CreateTestEntry(out string name, out string icon, out string color, out string description)
        {
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:floor";
            color = "red";
            description = "test description";
            return Label.CreateUnmodified(name, icon, color, description);
        }
    }
}
