using HassClient.Models;
using NUnit.Framework;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Device))]
    public class DeviceTests
    {
        [Test]
        public void HasNoPublicConstructors()
        {
            var constructor = typeof(Device).GetConstructors()
                                            .FirstOrDefault(x => x.IsPublic);
            Assert.Null(constructor);
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
        public void SetNewAreaIdMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialAreaId, out _);

            testEntry.AreaId = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.AreaId = initialAreaId;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialName, out var initialAreaId, out var initialDisabledBy);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.AreaId = MockHelpers.GetRandomTestName();
            //testEntry.DisabledBy = DisabledByEnum.User;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialAreaId, testEntry.AreaId);
            Assert.AreEqual(initialDisabledBy, testEntry.DisabledBy);
        }

        [Test]
        public void NameIsNameByUserIfDefined()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out _);

            Assert.AreEqual(testEntry.OriginalName, testEntry.Name);

            var testName = MockHelpers.GetRandomTestName();
            testEntry.Name = testName;
            Assert.AreEqual(testName, testEntry.Name);

            testEntry.Name = null;
            Assert.AreEqual(testEntry.OriginalName, testEntry.Name);
        }

        private Device CreateTestEntry(out string entityId, out string name, out string areaId, out DisabledByEnum disabledBy)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.Esphome);
            name = MockHelpers.GetRandomTestName();
            areaId = MockHelpers.GetRandomTestName();
            disabledBy = DisabledByEnum.Integration;
            return Device.CreateUnmodified(entityId, name, areaId, disabledBy);
        }
    }
}
