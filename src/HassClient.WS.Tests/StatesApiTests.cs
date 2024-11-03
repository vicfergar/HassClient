using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class StatesApiTests : BaseHassWSApiTest
    {
        private IEnumerable<StateModel> states;

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
            Assert.IsTrue(this.states.All(x => x.LastChanged > DateTimeOffset.MinValue));
        }

        [Test]
        public void GetStatesHasLastUpdated()
        {
            Assert.IsTrue(this.states.All(x => x.LastUpdated > DateTimeOffset.MinValue));
        }

        [Test]
        public void GetStatesHasLastReported()
        {
            Assert.IsTrue(this.states.All(x => x.LastReported > DateTimeOffset.MinValue));
        }

        [Test]
        public void GetStatesHasState()
        {
            Assert.IsTrue(this.states.All(x => !string.IsNullOrEmpty(x.State)));
        }

        [Test]
        public void GetStatesHasEntityId()
        {
            Assert.IsTrue(this.states.All(x => !string.IsNullOrEmpty(x.EntityId)));
        }

        [Test]
        public void GetStatesHasContext()
        {
            Assert.IsTrue(this.states.All(x => x.Context != null));
            Assert.IsTrue(this.states.All(x => !string.IsNullOrEmpty(x.Context.Id)));
        }

        [Test]
        public void GetStatesHasFriendlyName()
        {
            Assert.IsTrue(this.states.Any(x => !string.IsNullOrEmpty(x.FriendlyName)));
        }

        [Test]
        public void GetStatesHasSupportedFeatures()
        {
            Assert.IsTrue(this.states.Any(x => x.SupportedFeatures != null && x.SupportedFeatures > 0)); 
        }
    }
}
