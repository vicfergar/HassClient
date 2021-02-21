using HassClient.Models;

namespace HassClient.WS.Messages
{
    internal class UserResponse
    {
        public User User { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{this.User}";
    }
}
