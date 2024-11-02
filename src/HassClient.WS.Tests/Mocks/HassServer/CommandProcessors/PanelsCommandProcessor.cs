using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class PanelsCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetPanelsMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var objs = MockHassModelFactory.PanelInfoFaker.Generate(10).ToDistinctDictionary(x => x.ComponentName);
            var resultObject = new JRaw(HassSerializer.SerializeObject(objs));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
