using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(false, TestName = nameof(PersonApiTests) + "WithRealServer")]
    public class PersonApiTests : BaseHassWSApiTest
    {
        private Person testPerson;
        public PersonApiTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreatePerson()
        {
            if (this.testPerson == null)
            {
                var testUser = new User(MockHelpers.GetRandomTestName(), false);
                var result = await this.hassWSApi.CreateUserAsync(testUser);
                Assert.IsTrue(result, "SetUp failed");

                this.testPerson = new Person(testUser.Name, testUser);
                result = await this.hassWSApi.CreateStorageEntityRegistryEntryAsync(this.testPerson);
                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testPerson);
            Assert.NotNull(this.testPerson.UniqueId);
            Assert.NotNull(this.testPerson.Name);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
            Assert.IsTrue(this.testPerson.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetPersons()
        {
            var result = await this.hassWSApi.GetStorageEntityRegistryEntriesAsync<Person>();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains(this.testPerson));
            Assert.IsTrue(result.All(x => x.Id != null));
            Assert.IsTrue(result.All(x => x.UniqueId != null));
            Assert.IsTrue(result.All(x => x.EntityId.StartsWith("person.")));
            Assert.IsTrue(result.Any(x => x.Name != null));
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonName()
        {
            this.testPerson.Name = $"{nameof(PersonApiTests)}_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonPicture()
        {
            this.testPerson.Picture = "test/Picture.png";
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonDeviceTrackers()
        {
            this.testPerson.DeviceTrackers.Add($"device_tracker.{MockHelpers.GetRandomTestName()}");
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [Test, Order(3)]
        public async Task UpdatePersonUserId()
        {
            var testUser = new User(MockHelpers.GetRandomTestName(), false);
            var result = await this.hassWSApi.CreateUserAsync(testUser);
            Assert.IsTrue(result, "SetUp failed");

            this.testPerson.ChangeUser(testUser);
            result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testPerson);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = this.testPerson.Name;
            var clonedEntry = this.testPerson.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            var result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(this.testPerson.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.UpdateStorageEntityRegistryEntryAsync(this.testPerson, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, this.testPerson.Name);
            Assert.IsFalse(this.testPerson.HasPendingChanges);
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeletePerson()
        {
            if (this.testPerson == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteStorageEntityRegistryEntryAsync(this.testPerson);
            var deletedPerson = this.testPerson;
            this.testPerson = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedPerson.IsTracked);
        }
    }
}
