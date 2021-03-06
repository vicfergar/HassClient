using HassClient.Core.Tests;
using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(UserRegistryApiTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(UserRegistryApiTests) + "WithRealServer")]
    public class UserRegistryApiTests : BaseHassWSApiTest
    {
        private User testUser;

        public UserRegistryApiTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateUser()
        {
            if (this.testUser == null)
            {
                this.testUser = new User(MockHelpers.GetRandomTestName());
                var result = await this.hassWSApi.CreateUserAsync(this.testUser);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testUser.Id);
            Assert.NotNull(this.testUser.Name);
            Assert.IsTrue(this.testUser.IsActive);
            Assert.IsFalse(this.testUser.IsOwner);
            Assert.IsFalse(this.testUser.IsAdministrator);
            Assert.IsFalse(this.testUser.HasPendingChanges);
            Assert.IsTrue(this.testUser.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetUsers()
        {
            var users = await this.hassWSApi.GetUsersAsync();

            Assert.NotNull(users);
            Assert.IsNotEmpty(users);
            Assert.IsTrue(users.Contains(this.testUser));
            Assert.IsTrue(users.Any(u => u.IsOwner));
            Assert.IsTrue(users.Any(u => u.IsAdministrator));
        }

        [Test, Order(3)]
        public async Task UpdateUser()
        {
            this.testUser.Name = $"TestUser_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateUserAsync(this.testUser);

            Assert.IsTrue(result);
        }

        [Test, Order(3)]
        public async Task UpdateUserWithAdminGroupId()
        {
            this.testUser.IsAdministrator = true;
            var result = await this.hassWSApi.UpdateUserAsync(this.testUser);

            Assert.IsTrue(result);
            Assert.IsTrue(this.testUser.IsAdministrator);
        }

        [OneTimeTearDown]
        [Test, Order(4)]
        public async Task DeleteUser()
        {
            if (this.testUser == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteUserAsync(this.testUser);
            var deletedUser = this.testUser;
            this.testUser = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedUser.IsTracked);
        }
    }
}
