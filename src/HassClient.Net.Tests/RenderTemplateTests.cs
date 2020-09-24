using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
{
    [TestFixture(true, TestName = nameof(RenderTemplateTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(RenderTemplateTests) + "WithRealServer")]
    public class RenderTemplateTests : BaseHassWSApiTest
    {
        public RenderTemplateTests(bool useFakeHassServer)
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
