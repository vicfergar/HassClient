using HassClient.Models;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(InputBooleanTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(InputBooleanTests) + "WithRealServer")]
    public class InputBooleanTests : BaseHassWSApiTest
    {
        private InputBoolean testInputBoolean;

        public InputBooleanTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateInputBoolean()
        {
            if (this.testInputBoolean == null)
            {
                this.testInputBoolean = new InputBoolean() 
                { 
                    Name = $"{nameof(InputBooleanTests)}_{DateTime.Now.Ticks}",
                    Initial = true,
                    Icon = "mdi:fan",
                };
                var result = await this.hassWSApi.CreateInputBooleanAsync(this.testInputBoolean);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testInputBoolean);
            Assert.NotNull(this.testInputBoolean.UniqueId);
            Assert.NotNull(this.testInputBoolean.Name);
        }

        [Test, Order(2)]
        public async Task GetInputBooleans()
        {
            var result = await this.hassWSApi.GetInputBooleansAsync();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains(this.testInputBoolean));
            Assert.IsTrue(result.Any(x => x.Name != null));
            Assert.IsTrue(result.Any(x => x.Initial == true));
            Assert.IsTrue(result.Any(x => x.Icon != null));
        }

        [Test, Order(3)]
        public async Task UpdateInputBooleanName()
        {
            this.testInputBoolean.Name = $"{nameof(InputBooleanTests)}_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [Test, Order(4)]
        public async Task UpdateInputBooleanInitial()
        {
            this.testInputBoolean.Initial = false;
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [Test, Order(5)]
        public async Task UpdateInputBooleanIcon()
        {
            this.testInputBoolean.Icon = $"mdi:lightbulb";
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [OneTimeTearDown]
        [Test, Order(6)]
        public async Task DeleteInputBoolean()
        {
            if (this.testInputBoolean == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteInputBooleanAsync(this.testInputBoolean);
            this.testInputBoolean = null;

            Assert.IsTrue(result);
        }
    }
}
