using Bogus;
using HassClient.Net.Helpers;
using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using HassClient.Net.Tests.Mocks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class RenderTemplateCommandProcessor : BaseCommandProcessor
    {
        private HashSet<string> entityIds;

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) => receivedCommand is RenderTemplateMessage;

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            var commandRenderTemplate = receivedCommand as RenderTemplateMessage;

            this.entityIds = new HashSet<string>();
            var result = Regex.Replace(commandRenderTemplate.Template, @"{{ (.*) }}", this.RenderTemplateValue);
            var listeners = new ListenersTemplateInfo()
            {
                All = false,
                Time = false,
            };
            listeners.Entities = this.entityIds.ToArray();
            listeners.Domains = listeners.Entities.Select(x => x.GetDomain()).ToArray();
            this.entityIds = null;

            var renderTemplateEvent = new TemplateEventInfo()
            {
                Result = result,
                Listeners = listeners,
            };

            var eventMsg = new EventResultMessage()
            {
                Id = commandRenderTemplate.Id,
                Event = new JRaw(HassSerializer.SerializeObject(renderTemplateEvent))
            };
            Task.Factory.StartNew(async() =>
            {
                await Task.Delay(40);
                await context.SendMessageAsync(eventMsg, CancellationToken.None); 
            });

            return this.CreateResultMessageWithResult(null);
        }

        private string RenderTemplateValue(Match match)
        {
            var statesPattern = @"states\('(.*)'\)";
            return Regex.Replace(match.Groups[1].Value, statesPattern, this.RenderStateValue);
        }

        private string RenderStateValue(Match match)
        {
            var entityId = match.Groups[1].Value;
            this.entityIds.Add(entityId);

            if (entityId.GetDomain() == "sun")
            {
                return "below_horizon";
            }
            else
            {
                return (new Faker()).RandomEntityState();
            }
        }
    }
}
