using Bogus;
using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class SearchCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is SearchRelatedMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var searchMessage = receivedCommand as SearchRelatedMessage;
            var resultResponse = new SearchRelatedResponse();

            if (searchMessage.ItemType == ItemTypes.Entity &&
                searchMessage.ItemId == "weather.home")
            {
                var faker = new Faker();
                resultResponse.AreaIds = new[] { faker.RandomUUID() };
                resultResponse.AutomationIds = new[] { faker.RandomUUID() };
                resultResponse.ConfigEntryIds = new[] { faker.RandomUUID() };
                resultResponse.DeviceIds = new[] { faker.RandomUUID() };
                resultResponse.EntityIds = new[] { faker.RandomEntityId() };
            }

            var resultObject = new JRaw(HassSerializer.SerializeObject(resultResponse));
            return this.CreateResultMessageWithResult(resultObject);
        }
    }
}
