using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(UserRegistryTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(UserRegistryTests) + "WithRealServer")]
    public class UserRegistryTests : BaseHassWSApiTest
    {
        private User testUser;

        public UserRegistryTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateUser()
        {
            if (this.testUser == null)
            {
                this.testUser = await this.hassWSApi.CreateUserAsync(new User($"TestUser_{DateTime.Now.Ticks}"));
                return;
            }

            Assert.NotNull(this.testUser);
            Assert.NotNull(this.testUser.Id);
            Assert.NotNull(this.testUser.Name);
            Assert.IsTrue(this.testUser.IsActive);
            Assert.IsFalse(this.testUser.IsOwner);
            Assert.IsFalse(this.testUser.IsAdministrator);
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
            this.testUser = null;

            Assert.IsTrue(result);
        }
    }
}
