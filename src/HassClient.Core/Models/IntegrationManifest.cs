using Newtonsoft.Json;
using System;

namespace HassClient.Models
{
    /// <summary>
    /// Represents the manifest that specify basic information about an integration.
    /// </summary>
    public class IntegrationManifest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the integration is maintained by Home Assistant Core.
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// Gets or sets an unique short name consisting of characters and underscores (e.g. "mobile_app").
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets a friendly name for the integration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the website containing documentation on how to use the integration.
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Gets or sets the issue tracker where users reports issues if they run into one. This is only defined for not
        /// <see cref="IsBuiltIn"/> integrations.
        /// </summary>
        public string IssueTracker { get; set; }

        /// <summary>
        /// Gets or sets a list of other integrations that need to set up successfully prior to the integration being loaded.
        /// </summary>
        public string[] Dependencies { get; set; }

        /// <summary>
        /// Gets or sets a list of optional dependencies that might be used by this integration.
        /// </summary>
        public string[] AfterDependencies { get; set; }

        /// <summary>
        /// Gets or sets a list of GitHub usernames or team names of people that are responsible for this integration.
        /// </summary>
        public string[] Codeowners { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the integration has a configuration flow to create a config entry.
        /// </summary>
        public bool ConfigFlow { get; set; }

        /// <summary>
        /// Gets or sets a list of Python libraries or modules needed by this integration.
        /// </summary>
        public string[] Requirements { get; set; }

        /// <summary>
        /// Gets or sets an value that scores an integration on the code quality and user experience.
        /// <para>
        /// More info at <see href="https://developers.home-assistant.io/docs/integration_quality_scale_index"/>.
        /// </para>
        /// </summary>
        public string QualityScale { get; set; }

        /// <summary>
        /// Gets or sets the version number from which this integration is available or compatible.
        /// </summary>
        [JsonProperty("homeassistant")]
        public Version SinceHassVersion { get; set; }

        /// <inheritdoc />
        public override string ToString() => this.Name;
    }
}
