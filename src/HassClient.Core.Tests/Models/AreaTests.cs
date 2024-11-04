using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Area))]
    public class AreaTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Area).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewAreaHasPendingChanges()
        {
            var testEntry = new Area(MockHelpers.GetRandomTestName());
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewAreaIsUntracked()
        {
            var testEntry = new Area(MockHelpers.GetRandomTestName());
            Assert.False(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => NamedEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Area(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out _, out _, out _, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialIcon, out _, out _, out _, out _);

            testEntry.Icon = "mdi:lamp";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewPictureMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var _, out var initialPicture, out _, out _, out _);

            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Picture = initialPicture;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewFloorIdMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initialFloorId, out _, out _);

            testEntry.FloorId = "upper_floor";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.FloorId = initialFloorId;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewAliasesMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _, out var _, out _);

            testEntry.Aliases.Add("alias3");
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Aliases.Remove("alias3"); 
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewLabelsMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _, out _, out var _);

            testEntry.Labels.Add("label3");
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Labels.Remove("label3");
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out var initialIcon, out var initialPicture, out var initialFloorId, out var initialAliases, out var initialLabels);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            testEntry.Icon = "mdi:lamp";
            testEntry.FloorId = "upper_floor";
            testEntry.Aliases.Add("alias3");
            testEntry.Labels.Add("label3");
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initialPicture, testEntry.Picture);
            Assert.AreEqual(initialFloorId, testEntry.FloorId);
            Assert.AreEqual(initialAliases, testEntry.Aliases);
            Assert.AreEqual(initialLabels, testEntry.Labels);
        }

        private Area CreateTestEntry(out string name, out string icon, out string picture, out string floorId, out string[] aliases, out string[] labels)
        {
            name = MockHelpers.GetRandomTestName();
            picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            icon = "mdi:home";
            floorId = "ground_floor";
            aliases = new[] { "alias1", "alias2" };
            labels = new[] { "label1", "label2" };
            return Area.CreateUnmodified(name, icon, picture, floorId, aliases, labels);
        }
    }
}
