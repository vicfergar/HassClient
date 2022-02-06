using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents the manifest that specify basic information about an integration.
    /// </summary>
    public class IntegrationManifest
    {
        /// <summary>
        /// Gets a value indicating whether the integration is maintained by Home Assistant Core.
        /// </summary>
        [JsonProperty]
        public bool IsBuiltIn { get; private set; }

        /// <summary>
        /// Gets an unique short name consisting of characters and underscores (e.g. "mobile_app").
        /// </summary>
        [JsonProperty]
        public string Domain { get; private set; }

        /// <summary>
        /// Gets a friendly name for the integration.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the website containing documentation on how to use the integration.
        /// </summary>
        [JsonProperty]
        public string Documentation { get; private set; }

        /// <summary>
        /// Gets the issue tracker where users reports issues if they run into one. This is only defined for not
        /// <see cref="IsBuiltIn"/> integrations.
        /// </summary>
        [JsonProperty]
        public string IssueTracker { get; private set; }

        /// <summary>
        /// Gets a list of other integrations that need to set up successfully prior to the integration being loaded.
        /// </summary>
        [JsonProperty]
        public string[] Dependencies { get; private set; }

        /// <summary>
        /// Gets a list of optional dependencies that might be used by this integration.
        /// </summary>
        [JsonProperty]
        public string[] AfterDependencies { get; private set; }

        /// <summary>
        /// Gets a list of GitHub usernames or team names of people that are responsible for this integration.
        /// </summary>
        [JsonProperty]
        public string[] Codeowners { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the integration has a configuration flow to create a config entry.
        /// </summary>
        [JsonProperty]
        public bool ConfigFlow { get; private set; }

        /// <summary>
        /// Gets a list of Python libraries or modules needed by this integration.
        /// </summary>
        [JsonProperty]
        public string[] Requirements { get; private set; }

        /// <summary>
        /// Gets an value that scores an integration on the code quality and user experience.
        /// <para>
        /// More info at <see href="https://developers.home-assistant.io/docs/integration_quality_scale_index"/>.
        /// </para>
        /// </summary>
        [JsonProperty]
        public string QualityScale { get; private set; }

        /// <summary>
        /// Gets the version number from which this integration is available or compatible.
        /// </summary>
        [JsonProperty("homeassistant")]
        public CalVer SinceHassVersion { get; private set; }

        /// <inheritdoc />
        public override string ToString() => this.Name;
    }
}
