using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a signle service definition.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets or sets the description of the service object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fields/parameters that the service supports.
        /// </summary>
        public Dictionary<string, ServiceField> Fields { get; set; }
    }
}
