using HassClient.Models;
using NUnit.Framework;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(EntityRegistryEntry))]
    public class EntityRegistryEntryTests
    {
        [Test]
        public void HasNoPublicConstructors()
        {
            var constructor = typeof(EntityRegistryEntry)
                                    .GetConstructors()
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
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialIcon, out _);

            testEntry.Icon = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        /*[Test]
        public void SetDisableMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out _, out var initialDisabledBy);

            testEntry.DisabledBy = DisabledByEnum.User;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DisabledBy = initialDisabledBy;
            Assert.False(testEntry.HasPendingChanges);
        }*/

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialName, out var initialIcon, out var initialDisabledBy);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = MockHelpers.GetRandomTestName();
            //testEntry.DisabledBy = DisabledByEnum.User;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initialDisabledBy, testEntry.DisabledBy);
        }

        private EntityRegistryEntry CreateTestEntry(out string entityId, out string name, out string icon, out DisabledByEnum disabledBy)
        {
            entityId = MockHelpers.GetRandomEntityId(KnownDomains.InputBoolean);
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:camera";
            disabledBy = DisabledByEnum.Integration;
            return EntityRegistryEntry.CreateUnmodified(entityId, name, icon, disabledBy);
        }
    }
}
