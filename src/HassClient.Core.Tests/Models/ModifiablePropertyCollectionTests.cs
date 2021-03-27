using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(ModifiablePropertyCollection<string>))]
    public class ModifiablePropertyCollectionTests
    {
        [Test]
        public void DoesNotAcceptsDuplicatedValues()
        {
            var collectionProperty = this.CreateCollectionProperty();

            const string item = "Test";
            collectionProperty.Value.Add(item);
            collectionProperty.Value.Add(item);

            Assert.That(collectionProperty.Value, Has.Exactly(1).Matches<string>(p => p == item));
        }

        [Test]
        public void AddInvalidValueWhenUsingValidationFuncThrows()
        {
            var collectionProperty = this.CreateCollectionProperty(0, x => x == "Test");

            Assert.Throws<InvalidOperationException>(() => collectionProperty.Value.Add("Test2"));
        }

        [Test]
        public void AddValidValueWhenUsingValidationFuncDoesNotThrows()
        {
            const string item = "Test";
            var collectionProperty = this.CreateCollectionProperty(0, validationFunc: x => x == item);

            Assert.DoesNotThrow(() => collectionProperty.Value.Add(item));
        }

        [Test]
        public void SaveChangesMakesHasPendingChangesFalse()
        {
            var collectionProperty = this.CreateCollectionProperty(hasChanges: true);
            Assert.IsTrue(collectionProperty.HasPendingChanges);

            collectionProperty.SaveChanges();
            Assert.IsFalse(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChangesMakesHasPendingChangesFalse()
        {
            var collectionProperty = this.CreateCollectionProperty(hasChanges: true);
            Assert.IsTrue(collectionProperty.HasPendingChanges);

            collectionProperty.DiscardPendingChanges();
            Assert.IsFalse(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void DiscardPendingChangesRestoresPreviousValues()
        {
            var collectionProperty = this.CreateCollectionProperty(2);
            var initialValues = collectionProperty.Value.ToArray();

            collectionProperty.Value.Add("Test");
            collectionProperty.DiscardPendingChanges();

            CollectionAssert.AreEqual(initialValues, collectionProperty.Value);
        }

        [Test]
        public void AddNewValueMakesHasPendingChangesTrue()
        {
            var collectionProperty = this.CreateCollectionProperty();
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Add("Test");

            Assert.IsTrue(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void RemoveValueMakesHasPendingChangesTrue()
        {
            var collectionProperty = this.CreateCollectionProperty();
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Remove(collectionProperty.Value.First());

            Assert.IsTrue(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void ClearValuesMakesHasPendingChangesTrue()
        {
            var collectionProperty = this.CreateCollectionProperty(2);
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Clear();
            Assert.IsTrue(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void AddAndRemoveValueMakesHasPendingChangesFalse()
        {
            var collectionProperty = this.CreateCollectionProperty(2);
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            const string item = "Test";
            collectionProperty.Value.Add(item);
            Assert.IsTrue(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Remove(item);
            Assert.IsFalse(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void RemoveAndAddValueMakesHasPendingChangesFalse()
        {
            var collectionProperty = this.CreateCollectionProperty(2);
            var existingValue = collectionProperty.Value.First();
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Remove(existingValue);
            Assert.IsTrue(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Add(existingValue);
            Assert.IsFalse(collectionProperty.HasPendingChanges);
        }

        [Test]
        public void ClearAndAddSameValuesMakesHasPendingChangesFalse()
        {
            var collectionProperty = this.CreateCollectionProperty(2);
            var initialValues = collectionProperty.Value.ToArray();
            Assert.IsFalse(collectionProperty.HasPendingChanges);

            collectionProperty.Value.Clear();
            Assert.IsTrue(collectionProperty.HasPendingChanges);

            foreach (var item in initialValues)
            {
                collectionProperty.Value.Add(item);
            }
            Assert.IsFalse(collectionProperty.HasPendingChanges);
        }

        private ModifiablePropertyCollection<string> CreateCollectionProperty(int elementCount = 2, Func<string, bool> validationFunc = null, bool hasChanges = false, [CallerMemberName] string collectionName = null)
        {
            var collection = new ModifiablePropertyCollection<string>(collectionName, validationFunc);
            for (int i = 0; i < elementCount; i++)
            {
                collection.Value.Add(MockHelpers.GetRandomEntityId(KnownDomains.Climate));
            }

            if (!hasChanges)
            {
                collection.SaveChanges();
            }

            return collection;
        }
    }
}
