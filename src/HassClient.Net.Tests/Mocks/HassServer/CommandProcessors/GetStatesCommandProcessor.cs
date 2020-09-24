using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class GetStatesCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetStatesMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var states = MockHassModelFactory.StateModelFaker.Generate(30);
            var resultObject = new JRaw(HassSerializer.SerializeObject(states));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
