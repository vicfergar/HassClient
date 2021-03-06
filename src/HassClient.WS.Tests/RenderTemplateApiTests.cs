using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(RenderTemplateApiTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(RenderTemplateApiTests) + "WithRealServer")]
    public class RenderTemplateApiTests : BaseHassWSApiTest
    {
        public RenderTemplateApiTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public async Task RenderTemplate()
        {
            var result = await this.hassWSApi.RenderTemplateAsync("The sun is {{ states('sun.sun') }}");

            Assert.IsNotNull(result);
            Assert.IsTrue(Regex.IsMatch(result, "^The sun is (?:above_horizon|below_horizon)$"));
        }
    }
}
