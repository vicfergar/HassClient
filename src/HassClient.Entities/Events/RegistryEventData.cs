using Newtonsoft.Json.Linq;

namespace HassClient.Entities.Events
{
    internal class RegistryEventData
    {
        public enum Actions
        {
            Undefined,
            Create,
            Remove,
            Update,
        }

        public Actions Action { get; set; }

        public string AreaId { get; set; }

        public string DeviceId { get; set; }

        public string EntityId { get; set; }

        public string OldEntityId { get; set; }

        public string UserId { get; set; }

        public JRaw Changes { get; set; }
    }
}
