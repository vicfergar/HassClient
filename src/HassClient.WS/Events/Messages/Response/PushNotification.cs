using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Represents a push notification incoming message.
/// </summary>
public class PushNotification
{
    /// <summary>
    /// Gets the title of the push notification.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; set; }

    /// <summary>
    /// Gets the message of the push notification.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string Message { get; set; }

    /// <summary>
    /// Gets the data of the push notification.
    /// </summary>
    public Dictionary<string, object> Data { get; set; }

    /// <summary>
    /// Gets the confirmation ID for the push notification.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string HassConfirmId { get; set; }
}
