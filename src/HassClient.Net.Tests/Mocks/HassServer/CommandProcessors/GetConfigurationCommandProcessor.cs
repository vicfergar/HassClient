using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class GetConfigurationCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetConfigMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var configuration = MockHassModelFactory.ConfigurationFaker.Generate();
            var resultObject = new JRaw(HassSerializer.SerializeObject(configuration));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
