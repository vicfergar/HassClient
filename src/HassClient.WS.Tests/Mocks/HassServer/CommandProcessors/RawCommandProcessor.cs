using HassClient.WS.Messages;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class RawCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is RawCommandMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var rawCommand = (RawCommandMessage)receivedCommand;
            var messageType = rawCommand.Type;
            return new ResultMessage() { Success = true };
        }
    }
}
