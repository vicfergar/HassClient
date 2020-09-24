using HassClient.Net.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
{
    [TestFixture(true, TestName = nameof(StatesTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(StatesTests) + "WithRealServer")]
    public class StatesTests : BaseHassWSApiTest
    {
        private IEnumerable<StateModel> states;

        public StatesTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test]
        public async Task GetStates()
        {
            if (this.states != null)
            {
                return;
            }

            this.states = await this.hassWSApi.GetStatesAsync();

            Assert.IsNotNull(this.states);
            Assert.IsNotEmpty(this.states);
        }

        [Test]
        public void GetStatesHasAttributes()
        {
            Assert.IsTrue(this.states.All(x => x.Attributes.Count > 0));
        }

        [Test]
        public void GetStatesHasLastChanged()
        {
            Assert.IsTrue(this.states.All(x => x.LastChanged != default));
        }

        [Test]
        public void GetStatesHasLastUpdated()
        {
            Assert.IsTrue(this.states.All(x => x.LastUpdated != default));
        }

        [Test]
        public void GetStatesHasState()
        {
            Assert.IsTrue(this.states.All(x => x.State != default));
        }

        [Test]
        public void GetStatesHasEntityId()
        {
            Assert.IsTrue(this.states.All(x => x.EntityId != default));
        }

        [Test]
        public void GetStatesHasContext()
        {
            Assert.IsTrue(this.states.All(x => x.Context != default));
        }
    }
}
