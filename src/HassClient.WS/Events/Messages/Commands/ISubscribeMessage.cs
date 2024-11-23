using Newtonsoft.Json;

namespace HassClient.WS.Messages.Commands.Subscriptions
{
    /// <summary>
    /// Represents a message that creates a subscription.
    /// </summary>
    public interface ISubscribeMessage
    {
        /// <summary>
        /// The ID of the subscription.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Indicates whether the subscription is long-running.
        /// </summary>
        [JsonIgnore]
        bool IsLongRunning { get; }
    }
}
