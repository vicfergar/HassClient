using HassClient.Net.Models;
using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class CallServiceCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is CallServiceMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var callServiceMsg = receivedCommand as CallServiceMessage;
            var state = new StateModel()
            {
                Context = MockHassModelFactory.ContextFaker.Generate()
            };
            var resultObject = new JRaw(HassSerializer.SerializeObject(state));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
