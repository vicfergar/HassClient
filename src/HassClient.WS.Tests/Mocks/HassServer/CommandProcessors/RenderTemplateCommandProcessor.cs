using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class RenderTemplateCommandProcessor : BaseCommandProcessor
    {
        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => 
            receivedCommand is RenderTemplateMessage;

        public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var templateMessage = (RenderTemplateMessage)receivedCommand;
            
            var eventMsg = new IncomingEventMessage
            {
                Id = templateMessage.Id,
                Event = new JRaw(HassSerializer.SerializeObject(new TemplateEventInfo
                {
                    Result = "mocked_template_result",
                    Listeners = new ListenersTemplateInfo
                    {
                        All = false,
                        Time = false,
                        Entities = new[] { "light.living_room", "switch.kitchen" },
                        Domains = new[] { "light", "switch" }
                    }
                }))
            };

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(40);
                await context.SendMessageAsync(eventMsg, CancellationToken.None);
            });

            return CreateResultMessageWithResult(null);
        }
    }
}
