using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class RenderTemplateApiTests : BaseHassWSApiTest
    {
        [Test]
        public async Task RenderTemplate()
        {
            var result = await this.hassWSApi.RenderTemplateAsync("The sun is {{ states('sun.sun') }}");

            Assert.IsNotNull(result);
            Assert.IsTrue(Regex.IsMatch(result, "^The sun is (?:above_horizon|below_horizon)$"));
        }
    }
}
