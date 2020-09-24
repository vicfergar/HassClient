using HassClient.Net.WSMessages;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class RawCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is RawCommandMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var rawCommand = (RawCommandMessage)receivedCommand;
            var messageType = rawCommand.Type;
            return new ResultMessage() { Success = true };
        }
    }
}
