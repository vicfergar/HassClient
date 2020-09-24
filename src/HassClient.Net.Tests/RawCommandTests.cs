using HassClient.Net.WSMessages;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
{
    [TestFixture(true, TestName = nameof(RawCommandTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(RawCommandTests) + "WithRealServer")]
    public class RawCommandTests : BaseHassWSApiTest
    {
        public RawCommandTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [Test]
        public async Task SendRawCommandWithResult()
        {
            var result = await this.hassWSApi.SendRawCommandWithResultAsync(new RawCommandMessage("get_config"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Result);
        }

        [Test]
        public async Task SendRawCommandWithSuccess()
        {
            var result = await this.hassWSApi.SendRawCommandWithSuccessAsync(new RawCommandMessage("get_config"));

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendInvalidRawCommandWithResultReturnsUnknownCommandError()
        {
            var result = await this.hassWSApi.SendRawCommandWithResultAsync(new RawCommandMessage("invalid_command"));

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Result);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCodes.UnknownCommand, result.Error.Code);
        }

        [Test]
        public async Task PingPongCommandResponseIsEncapsulatedAsResultMessage()
        {
            var result = await this.hassWSApi.SendRawCommandWithResultAsync(new RawCommandMessage("ping"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }
    }
}
