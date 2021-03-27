using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Zone))]
    public class ZoneTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Zone).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewZoneHasPendingChanges()
        {
            var testEntry = new Zone(MockHelpers.GetRandomTestName(), 20, 30, 5);
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewZoneIsUntracked()
        {
            var testEntry = new Zone(MockHelpers.GetRandomTestName(), 20, 30, 5);
            Assert.IsFalse(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewZoneWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Zone(value, 20, 30, 5));
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

            testEntry.Icon = "mdi:test";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewLongitudeMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialLongitude, out _, out _, out _);

            testEntry.Longitude += 10;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Longitude = initialLongitude;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewLatitudeMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initialLatitude, out _, out _);

            testEntry.Latitude += 10;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Latitude = initialLatitude;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewRadiusMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _, out var initialRadius, out _);

            testEntry.Radius += 10;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Radius = initialRadius;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIsPassiveMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _, out _, out var initialIsPassive);

            testEntry.IsPassive = !initialIsPassive;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.IsPassive = initialIsPassive;
            Assert.False(testEntry.HasPendingChanges);
        }

        private Zone CreateTestEntry(out string name, out string icon, out float longitude, out float latitude, out float radius, out bool isPassive)
        {
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:zone";
            longitude = 20;
            latitude = 30;
            radius = 5;
            isPassive = false;
            return Zone.CreateUnmodified("testId", name, longitude, latitude, radius, icon, isPassive);
        }
    }
}
