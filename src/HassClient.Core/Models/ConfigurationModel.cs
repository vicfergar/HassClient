using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents the Home Assistant configuration object.
    /// </summary>
    public class ConfigurationModel
    {
        [JsonProperty("whitelist_external_dirs")]
        private string[] whitelist_external_dirs
        {
            get => this.AllowedExternalDirs?.ToArray();
            set => this.AllowedExternalDirs = this.AllowedExternalDirs ?? value.ToList();
        }

        [JsonProperty("allowlist_external_dirs")]
        private string[] allowlist_external_dirs
        {
            get => this.AllowedExternalDirs?.ToArray();
            set => this.AllowedExternalDirs = this.AllowedExternalDirs ?? value.ToList();
        }

        /// <summary>
        /// Gets the latitude of the current location.
        /// </summary>
        [JsonProperty]
        public float Latitude { get; private set; }

        /// <summary>
        /// Gets the longitude of the current location.
        /// </summary>
        [JsonProperty]
        public float Longitude { get; private set; }

        /// <summary>
        /// Gets the altitude above sea level in meters of the current location.
        /// </summary>
        [JsonProperty]
        public int Elevation { get; private set; }

        /// <summary>
        /// Gets a container for units of measure.
        /// </summary>
        [JsonProperty]
        public UnitSystemModel UnitSystem { get; private set; }

        /// <summary>
        /// Gets the location's friendly name.
        /// </summary>
        [JsonProperty]
        public string LocationName { get; private set; }

        /// <summary>
        /// Gets the time zone name (column "TZ" from <see href="https://en.wikipedia.org/wiki/List_of_tz_database_time_zones"/>).
        /// </summary>
        [JsonProperty]
        public string TimeZone { get; private set; }

        /// <summary>
        /// Gets the list of components loaded, in the [domain] or [domain].[component] format.
        /// </summary>
        [JsonProperty]
        public List<string> Components { get; private set; }

        /// <summary>
        /// Gets the relative path to the configuration directory (usually "/config").
        /// </summary>
        [JsonProperty("config_dir")]
        public string ConfigDirectory { get; private set; }

        /// <summary>
        /// Gets the list of folders that can be used as sources for sending files. (e.g. /config/www).
        /// </summary>
        [JsonIgnore]
        public List<string> AllowedExternalDirs { get; private set; }

        /// <summary>
        /// Gets the list of external URLs that can be fetched.
        /// <para>URLs can match specific resources (e.g., "http://10.10.10.12/images/image1.jpg") or a relative path that allows
        /// access to resources within it (e.g., "http://10.10.10.12/images" would allow access to anything under that path).
        /// </para>
        /// </summary>
        [JsonProperty("allowlist_external_urls")]
        public List<string> AllowedExternalUrls { get; private set; }

        /// <summary>
        /// Gets the version of Home Assistant that is currently running.
        /// </summary>
        [JsonProperty]
        public CalVer Version { get; private set; }

        /// <summary>
        /// Gets the configuration source, or type of configuration file (usually "storage").
        /// </summary>
        [JsonProperty]
        public string ConfigSource { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Home Assistant is running in safe mode.
        /// </summary>
        [JsonProperty]
        public bool SafeMode { get; private set; }

        /// <summary>
        /// Gets the current state of Home Assistant (usually "RUNNING").
        /// </summary>
        [JsonProperty]
        public string State { get; private set; }

        /// <summary>
        /// Gets the URL that Home Assistant is available on from the Internet (e.g. "https://example.duckdns.org:8123").
        /// </summary>
        [JsonProperty]
        public string ExternalUrl { get; private set; }

        /// <summary>
        /// Gets the URL that Home Assistant is available on from the local network (e.g. "http://homeassistant.local:8123").
        /// </summary>
        [JsonProperty]
        public string InternalUrl { get; private set; }

        /// <summary>
        /// Gets the currency code according to ISO 4217 (column "Code" from <see href="https://en.wikipedia.org/wiki/ISO_4217#Active_codes"/>).
        /// </summary>
        public string Currency { get; private set; }

        /// <inheritdoc />
        public override string ToString() => this.LocationName;
    }
}
