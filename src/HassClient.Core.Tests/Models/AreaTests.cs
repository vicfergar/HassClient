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

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Area(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewPictureMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var picture);

            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Picture = picture;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out var initialPicture);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialPicture, testEntry.Picture);
        }

        private Area CreateTestEntry(out string name, out string picture)
        {
            name = MockHelpers.GetRandomTestName();
            picture = "/test/Picture.png";
            return Area.CreateUnmodified(name, picture);
        }
    }
}
