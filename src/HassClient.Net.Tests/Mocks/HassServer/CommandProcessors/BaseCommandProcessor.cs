using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public abstract class BaseCommandProcessor
    {
        public abstract bool CanProcess(BaseIdentifiableMessage receivedCommand);

        public abstract BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand);

        protected BaseIdentifiableMessage CreateResultMessageWithError(ErrorInfo errorInfo) => new ResultMessage() { Error = errorInfo };

        protected BaseIdentifiableMessage CreateResultMessageWithResult(JRaw result) => new ResultMessage() { Success = true, Result = result };

        protected Stream GetResourceStream(string filename)
        {
            var assembly = typeof(BaseCommandProcessor).Assembly;
            return assembly.GetManifestResourceStream($"HassClient.Net.Tests.Mocks.Data.{filename}");
        }
    }
}
