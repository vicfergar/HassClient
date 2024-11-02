using HassClient.WS.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class GetServicesCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is GetServicesMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            using (var stream = this.GetResourceStream("GetServicesResponse.json"))
            using (var sr = new StreamReader(stream))
            using (var reader = new JsonTextReader(sr))
            {
                var resultObject = JRaw.Create(reader);
                return this.CreateResultMessageWithResult(resultObject);
            }
        }
    }
}
