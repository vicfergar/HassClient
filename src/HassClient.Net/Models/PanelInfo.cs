using HassClient.Net.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Net.Models
{
    /// <summary>
    /// Defines information related with a Home Assistant UI panel.
    /// </summary>
    public class PanelInfo
    {
        /// <summary>
        /// Gets or sets the panel component name. Typical values are <c>config</c>, <c>lovelace</c>, <c>custom</c>, etc.
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// Gets or sets the icon to display in the front-end.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the title to display in the front-end.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an object that contains specific configuration parameters of the panel.
        /// </summary>
        [JsonProperty("config")]
        public JRaw Configuration { get; set; }

        /// <summary>
        /// Gets or sets the URL path of the panel from which it can be accessed.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a user needs administrator rights to access the panel.
        /// </summary>
        public bool RequireAdmin { get; set; }

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
