using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Serialization
{
    /// <summary>
    /// Contract resolver used to filter only selected properties during object serialization.
    /// </summary>
    public class SelectedPropertiesContractResolver : DefaultContractResolver
    {
        private HashSet<string> selectedProperties;

        /// <summary>
        /// White-list containing the named of the properties to be included in the serialization.
        /// </summary>
        public IEnumerable<string> SelectedProperties
        {
            get => this.selectedProperties;
            set => this.selectedProperties = new HashSet<string>(value);
        }

        /// <inheritdoc />
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProps = base.CreateProperties(type, memberSerialization);
            return allProps.Where(p => this.selectedProperties.Contains(p.UnderlyingName)).ToList();
        }
    }
}
