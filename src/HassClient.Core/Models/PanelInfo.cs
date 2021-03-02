using HassClient.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Defines information related with a Home Assistant UI panel.
    /// </summary>
    public class PanelInfo
    {
        /// <summary>
        /// Gets the panel component name. Typical values are <c>config</c>, <c>lovelace</c>, <c>custom</c>, etc.
        /// </summary>
        [JsonProperty]
        public string ComponentName { get; private set; }

        /// <summary>
        /// Gets the icon to display in the front-end.
        /// </summary>
        [JsonProperty]
        public string Icon { get; private set; }

        /// <summary>
        /// Gets the title to display in the front-end.
        /// </summary>
        [JsonProperty]
        public string Title { get; private set; }

        /// <summary>
        /// Gets an object that contains specific configuration parameters of the panel.
        /// </summary>
        [JsonProperty("config")]
        public JRaw Configuration { get; private set; }

        /// <summary>
        /// Gets the URL path of the panel from which it can be accessed.
        /// </summary>
        [JsonProperty]
        public string UrlPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a user needs administrator rights to access the panel.
        /// </summary>
        [JsonProperty]
        public bool RequireAdmin { get; private set; }

        /// <summary>
        /// Deserializes the <see cref="Configuration"/> object with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized configuration object.</returns>
        public T DeserializeConfig<T>()
        {
            return HassSerializer.DeserializeObject<T>(this.Configuration);
        }

        /// <inheritdoc />
        public override string ToString() => this.Title ?? this.UrlPath;
    }
}
