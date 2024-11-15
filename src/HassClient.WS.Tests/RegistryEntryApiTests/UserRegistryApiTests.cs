using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class UserRegistryApiTests : BaseHassWSApiTest
    {
        private User testUser;

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateUser()
        {
            if (this.testUser == null)
            {
                this.testUser = new User(MockHelpers.GetRandomTestName());
                var result = await this.hassWSApi.Users.CreateAsync(this.testUser);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testUser.Id);
            Assert.NotNull(this.testUser.Name);
            Assert.IsTrue(this.testUser.IsActive);
            Assert.IsFalse(this.testUser.IsLocalOnly);
            Assert.IsFalse(this.testUser.IsOwner);
            Assert.IsFalse(this.testUser.IsAdministrator);
            Assert.IsFalse(this.testUser.HasPendingChanges);
            Assert.IsTrue(this.testUser.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetUsers()
        {
            var users = await this.hassWSApi.Users.ListAsync();

            Assert.NotNull(users);
            Assert.IsNotEmpty(users);
            Assert.IsTrue(users.Contains(this.testUser));
            Assert.IsTrue(users.Any(u => u.IsOwner));
            Assert.IsTrue(users.Any(u => u.IsAdministrator));
        }

        [Test, Order(3)]
        public async Task UpdateUserName()
        {
            var updatedName = MockHelpers.GetRandomTestName();
            this.testUser.Name = updatedName;
            var result = await this.hassWSApi.Users.UpdateAsync(this.testUser);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedName, this.testUser.Name);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsActive()
        {
            this.testUser.IsActive = false;
            var result = await this.hassWSApi.Users.UpdateAsync(this.testUser);

            Assert.IsTrue(result);
            Assert.IsFalse(this.testUser.IsActive);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsLocalOnly()
        {
            this.testUser.IsLocalOnly = true;
            var result = await this.hassWSApi.Users.UpdateAsync(this.testUser);

            Assert.IsTrue(result);
            Assert.IsTrue(this.testUser.IsLocalOnly);
        }

        [Test, Order(3)]
        public async Task UpdateUserIsAdministrator()
        {
            this.testUser.IsAdministrator = true;
            var result = await this.hassWSApi.Users.UpdateAsync(this.testUser);

            Assert.IsTrue(result);
            Assert.IsTrue(this.testUser.IsAdministrator);
        }

        [Test, Order(4)]
        public async Task UpdateWithForce()
        {
            var initialName = this.testUser.Name;
            var initialGroupIds = this.testUser.GroupIds;
            var initialIsActive = this.testUser.IsActive;
            var initialIsLocalOnly = this.testUser.IsLocalOnly;
            var clonedEntry = this.testUser.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.IsAdministrator = !this.testUser.IsAdministrator;
            clonedEntry.IsActive = !initialIsActive;
            clonedEntry.IsLocalOnly = !initialIsLocalOnly;
            var result = await this.hassWSApi.Users.UpdateAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(this.testUser.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.Users.UpdateAsync(this.testUser, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, this.testUser.Name);
            Assert.AreEqual(initialGroupIds, this.testUser.GroupIds);
            Assert.AreEqual(initialIsActive, this.testUser.IsActive);
            Assert.AreEqual(initialIsLocalOnly, this.testUser.IsLocalOnly);
        }

        [OneTimeTearDown]
        [Test, Order(5)]
        public async Task DeleteUser()
        {
            if (this.testUser == null)
            {
                return;
            }

            var result = await this.hassWSApi.Users.DeleteAsync(this.testUser);
            var deletedUser = this.testUser;
            this.testUser = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedUser.IsTracked);
        }
    }
}
