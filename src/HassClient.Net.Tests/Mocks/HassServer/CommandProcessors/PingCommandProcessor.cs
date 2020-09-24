using HassClient.Net.WSMessages;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class PingCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is PingMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            return new PongMessage();
        }
    }
}
