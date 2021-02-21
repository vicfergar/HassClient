using HassClient.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(PanelsTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(PanelsTests) + "WithRealServer")]
    public class PanelsTests : BaseHassWSApiTest
    {

        private IEnumerable<PanelInfo> panels;

        public PanelsTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test]
        public async Task GetPanels()
        {
            if (this.panels != null)
            {
                return;
            }

            this.panels = await this.hassWSApi.GetPanelsAsync();

            Assert.IsNotNull(this.panels);
            Assert.IsNotEmpty(this.panels);
        }

        [Test]
        public async Task GetPanel()
        {
            if (this.panels != null)
            {
                return;
            }

            var firstPanel = this.panels?.FirstOrDefault();
            Assert.NotNull(firstPanel, "SetUp failed");

            var result = await this.hassWSApi.GetPanelAsync(firstPanel.UrlPath);

            Assert.IsNotNull(result);
            Assert.AreEqual(firstPanel, result);
        }

        [Test]
        public void GetPanelWithNullUrlPathThrows()
        {
            Assert.ThrowsAsync<ArgumentException>(() => this.hassWSApi.GetPanelAsync(null));
        }

        [Test]
        public void GetPanelsHasComponentName()
        {
            Assert.IsTrue(this.panels.All(x => x.ComponentName != default));
        }

        [Test]
        public void GetPanelsHasConfiguration()
        {
            Assert.IsTrue(this.panels.All(x => x.Configuration != default));
        }

        [Test]
        public void GetPanelsHasIcon()
        {
            Assert.IsTrue(this.panels.Any(x => x.Icon != default));
        }

        [Test]
        public void GetPanelsHasRequireAdmin()
        {
            Assert.IsTrue(this.panels.Any(x => x.RequireAdmin == true));
        }

        [Test]
        public void GetPanelsHasTitle()
        {
            Assert.IsTrue(this.panels.Any(x => x.Title != default));
        }

        [Test]
        public void GetPanelsHasUrlPath()
        {
            Assert.IsTrue(this.panels.All(x => x.UrlPath != default));
        }
    }
}
