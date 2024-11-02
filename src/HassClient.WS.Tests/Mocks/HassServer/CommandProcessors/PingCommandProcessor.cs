using HassClient.WS.Messages;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class PingCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is PingMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            return new PongMessage();
        }
    }
}
