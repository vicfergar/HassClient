using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Models
{
    /// <summary>
    /// Represents the Home Assistant configuration object.
    /// </summary>
    public class Configuration
    {
        [JsonProperty("whitelist_external_dirs")]
        private string[] whitelist_external_dirs
        {
            get => this.AllowedExternalDirs?.ToArray();
            set => this.AllowedExternalDirs ??= value.ToList();
        }

        [JsonProperty("allowlist_external_dirs")]
        private string[] allowlist_external_dirs
        {
            get => this.AllowedExternalDirs?.ToArray();
            set => this.AllowedExternalDirs ??= value.ToList();
        }

        /// <summary>
        /// Gets or sets the latitude of the current location.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the current location.
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude above sea level in meters of the current location.
        /// </summary>
        public int Elevation { get; set; }

        /// <summary>
        /// Gets or sets a container for units of measure.
        /// </summary>
        public UnitSystemModel UnitSystem { get; set; }

        /// <summary>
        /// Gets or sets the location's friendly name.
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Gets or sets the time zone name (column "TZ" from <see href="https://en.wikipedia.org/wiki/List_of_tz_database_time_zones"/>).
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the list of components loaded, in the [domain] or [domain].[component] format.
        /// </summary>
        public List<string> Components { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the configuration directory (usually "/config").
        /// </summary>
        [JsonProperty("config_dir")]
        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Gets or sets the list of folders that can be used as sources for sending files. (e.g. /config/www).
        /// </summary>
        [JsonIgnore]
        public List<string> AllowedExternalDirs { get; set; }

        /// <summary>
        /// Gets or sets the list of external URLs that can be fetched.
        /// <para>URLs can match specific resources (e.g., "http://10.10.10.12/images/image1.jpg") or a relative path that allows
        /// access to resources within it (e.g., "http://10.10.10.12/images" would allow access to anything under that path).
        /// </para>
        /// </summary>
        [JsonProperty("allowlist_external_urls")]
        public List<string> AllowedExternalUrls { get; set; }

        /// <summary>
        /// Gets or sets the version of Home Assistant that is currently running (e.g. "0.115.3").
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the configuration source, or type of configuration file (usually "storage").
        /// </summary>
        public string ConfigSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Home Assistant is running in safe mode.
        /// </summary>
        public bool SafeMode { get; set; }

        /// <summary>
        /// Gets or sets the current state of Home Assistant (usually "RUNNING").
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the URL that Home Assistant is available on from the Internet (e.g. "https://example.duckdns.org:8123").
        /// </summary>
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL that Home Assistant is available on from the local network (e.g. "http://homeassistant.local:8123").
        /// </summary>
        public string InternalUrl { get; set; }

        /// <inheritdoc />
        public override string ToString() => this.LocationName;
    }
}
