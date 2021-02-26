using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a Home Assistant user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The System Administrator group id constant.
        /// </summary>
        public const string SYSADMIN_GROUP_ID = "system-admin";

        /// <summary>
        /// Gets or sets the ID of this user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user is owner of the system. In this case, the user will have full access to everything.
        /// </summary>
        [JsonProperty]
        public bool IsOwner { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is active.
        /// </summary>
        [JsonProperty]
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is administrator.
        /// </summary>
        [JsonIgnore]
        public bool IsAdministrator
        {
            get => this.GroupIds?.Contains(SYSADMIN_GROUP_ID) == true;
            set
            {
                if (value)
                {
                    this.GroupIds.Add(SYSADMIN_GROUP_ID);
                }
                else
                {
                    this.GroupIds.Remove(SYSADMIN_GROUP_ID);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user has been generated automatically by the system.
        /// </summary>
        public bool SystemGenerated { get; set; }

        /// <summary>
        /// Gets or sets a set of group ids where the user is included.
        /// </summary>
        public HashSet<string> GroupIds { get; set; }

        /// <summary>
        /// The credentials of this user.
        /// </summary>
        public JRaw Credentials { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.GroupIds = new HashSet<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        public User(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// This constructor is used for testing purposes only.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="isOwner">A value indicating whether the new user isOwner.</param>
        internal User(string name, bool isOwner)
           : this(name)
        {
            this.IsActive = true;
            this.IsOwner = isOwner;
            this.IsAdministrator = isOwner;
        }

        internal void SetIsActive(bool value)
        {
            this.IsActive = value;
        }

        /// <summary>
        /// Method used by the serializer to determine if the <see cref="GroupIds"/> property should be serialized.
        /// </summary>
        /// <returns><see langword="true"/> if the property should be serialized; otherwise, <see langword="false"/>.</returns>
        protected bool ShouldSerializeGroupIds() => this.GroupIds?.Count > 0;

        /// <inheritdoc />
        public override string ToString() => $"{nameof(User)}: {this.Name}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is User user &&
                   this.Id == user.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id);
        }
    }
}
