using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class EntitySourceCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is EntitySourceMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var commandEntitySource = receivedCommand as EntitySourceMessage;
            IEnumerable<EntitySource> objs;
            if (commandEntitySource.EntityIds?.Count() > 0)
            {
                objs = MockHassModelFactory.EntitySourceFaker.GenerateWithEntityIds(commandEntitySource.EntityIds);
            }
            else
            {
                objs = MockHassModelFactory.EntitySourceFaker.Generate(10);
            }

            var resultObject = new JRaw(HassSerializer.SerializeObject(objs.ToDistinctDictionary(x => x.EntityId)));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
