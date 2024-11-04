using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(Category))]
    public class CategoryTests
    {
        [Test]
        public void HasPublicConstructorWithParameters()
        {
            var constructor = typeof(Category).GetConstructors()
                                          .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
            Assert.NotNull(constructor);
        }

        [Test]
        public void NewCategoryHasPendingChanges()
        {
            var testEntry = new Category(MockHelpers.GetRandomTestName(), "test_scope");
            Assert.IsTrue(testEntry.HasPendingChanges);
        }

        [Test]
        public void NewCategoryIsUntracked()
        {
            var testEntry = new Category(MockHelpers.GetRandomTestName(), "test_scope");
            Assert.False(testEntry.IsTracked);
        }

        private static IEnumerable<string> NullOrWhiteSpaceStringValues() => NamedEntryBaseTests.NullOrWhiteSpaceStringValues();

        [Test]
        [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
        public void NewCategoryWithNullOrWhiteSpaceNameThrows(string value)
        {
            Assert.Throws<ArgumentException>(() => new Category(value, "test_scope"));
        }

        [Test]
        public void SetNewNameMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out _, out _);

            testEntry.Name = MockHelpers.GetRandomTestName();
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Name = initialName;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void SetNewIconMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out var initialIcon, out _);

            testEntry.Icon = "mdi:lamp";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Icon = initialIcon;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]  
        public void SetNewScopeMakesHasPendingChangesTrue()
        {
            var testEntry = this.CreateTestEntry(out _, out _, out var initialScope);

            testEntry.Scope = "new_scope";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.Scope = initialScope;
            Assert.False(testEntry.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChanges()
        {
            var testEntry = this.CreateTestEntry(out var initialName, out var initialIcon, out var initialScope);

            testEntry.Name = MockHelpers.GetRandomTestName();
            testEntry.Icon = "mdi:lamp";
            testEntry.Scope = "new_scope";
            Assert.IsTrue(testEntry.HasPendingChanges);

            testEntry.DiscardPendingChanges();
            Assert.False(testEntry.HasPendingChanges);
            Assert.AreEqual(initialName, testEntry.Name);
            Assert.AreEqual(initialIcon, testEntry.Icon);
            Assert.AreEqual(initialScope, testEntry.Scope);
        }

        private Category CreateTestEntry(out string name, out string icon, out string scope)
        {
            name = MockHelpers.GetRandomTestName();
            icon = "mdi:floor";
            scope = "initial_scope";
            return Category.CreateUnmodified(name, scope, icon);
        }
    }
}
