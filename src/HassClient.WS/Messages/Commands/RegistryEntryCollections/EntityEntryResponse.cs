using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WS.Messages
{
    internal class EntityEntryResponse
    {
        [JsonProperty("entity_entry")]
        public JRaw EntityEntryRaw { get; set; }

        [JsonProperty]
        public int ReloadDelay { get; set; }

        [JsonProperty]
        public bool RequireRestart { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{this.EntityEntryRaw}";
    }
}
