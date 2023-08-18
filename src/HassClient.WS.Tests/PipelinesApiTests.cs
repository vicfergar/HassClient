using HassClient.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class PipelinesApiTests : BaseHassWSApiTest
    {
        private PipelineList pipelines;

        [OneTimeSetUp]
        [Test]
        public async Task GetPipelines()
        {
            if (this.pipelines != null)
            {
                return;
            }

            this.pipelines = await this.hassWSApi.GetPipelinesListAsync();

            Assert.IsNotNull(this.pipelines);
        }
    }
}
