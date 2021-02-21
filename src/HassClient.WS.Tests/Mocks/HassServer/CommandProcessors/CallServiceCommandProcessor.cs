using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
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
