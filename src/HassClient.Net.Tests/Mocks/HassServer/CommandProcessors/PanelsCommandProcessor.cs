using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class PanelsCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetPanelsMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var objs = MockHassModelFactory.PanelInfoFaker.Generate(10).ToDistinctDictionary(x => x.ComponentName);
            var resultObject = new JRaw(HassSerializer.SerializeObject(objs));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
