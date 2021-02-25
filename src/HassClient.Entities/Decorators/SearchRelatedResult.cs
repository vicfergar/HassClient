using HassClient.Models;
using HassClient.WS.Messages;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents a result from <see cref="Entity.SearchRelatedAsync(System.Threading.CancellationToken)"/>.
    /// It contains relations between things like <see cref="Areas"/>, <see cref="Devices"/>, <see cref="Entities"/>,
    /// <see cref="ConfigEntries"/>, scenes, scripts and <see cref="Automations"/>.
    /// </summary>
    public class SearchRelatedResult
    {
        /// <summary>
        /// Areas related with the search target entity.
        /// </summary>
        public IEnumerable<Area> Areas { get; private set; }

        /// <summary>
        /// Automations related with the search target entity.
        /// </summary>
        public IEnumerable<AutomationEntity> Automations { get; private set; }

        /// <summary>
        /// Configuration entries related with the search target entity.
        /// </summary>
        public IEnumerable<string> ConfigEntries { get; private set; }

        /// <summary>
        /// Devices related with the search target entity.
        /// </summary>
        public IEnumerable<Device> Devices { get; private set; }

        /// <summary>
        /// Entities related with the search target entity.
        /// </summary>
        public IEnumerable<Entity> Entities { get; private set; }

        internal SearchRelatedResult(SearchRelatedResponse relatedResponse, HassInstance hassInstance)
        {
            this.Areas = relatedResponse.AreaIds?
                                        .Select(id => hassInstance.Areas.FindById(id)) ??
                                        Enumerable.Empty<Area>();

            this.Automations = relatedResponse.AutomationIds?
                                          .Select(id => hassInstance.Entities.FindById<AutomationEntity>(id)) ??
                                           Enumerable.Empty<AutomationEntity>();

            this.ConfigEntries = relatedResponse.ConfigEntryIds ??
                                          Enumerable.Empty<string>();

            this.Devices = relatedResponse.DeviceIds?
                                          .Select(id => hassInstance.Devices.FindById(id)) ??
                                          Enumerable.Empty<Device>();

            this.Entities = relatedResponse.EntityIds?
                                           .Select(id => hassInstance.Entities.FindById(id)) ??
                                           Enumerable.Empty<Entity>();
        }
    }
}
