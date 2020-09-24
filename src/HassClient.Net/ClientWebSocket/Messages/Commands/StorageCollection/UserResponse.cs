using HassClient.Net.Models;

namespace HassClient.Net.WSMessages
{
    internal class UserResponse
    {
        public User User { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{this.User}";
    }
}
