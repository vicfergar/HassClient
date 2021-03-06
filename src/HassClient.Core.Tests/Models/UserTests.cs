using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(User))]
    public class UserTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(User).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewUserHasPendingChanges()
        {
            var testEntry = new User(MockHelpers.GetRandomTestName());
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewUserIsUntracked()
        {
            var testEntry = new User(MockHelpers.GetRandomTestName());
            Assert.IsFalse(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewUserWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new User(value));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out var initialName);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIsAdministratorMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _);

            testEntry.IsAdministrator = true;
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.IsAdministrator = false;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void AddNewGroupIdMakesHasPendingChangesTrue()
        {
            var testGroupId = "TestGroupId";
            var testEntry = this.CreateTestEntry(out _);

            testEntry.GroupIds.Add(testGroupId);
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.GroupIds.Remove(testGroupId);
            Assert.False(testEntry.HasPendingChanges);
        }

        private User CreateTestEntry(out string name)
        {
            name = MockHelpers.GetRandomTestName();
            return User.CreateUnmodified(name, false);
        }
    }
}
