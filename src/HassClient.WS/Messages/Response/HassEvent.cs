﻿using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Represents an event coming from the home assistant event bus.
    /// </summary>
    public class HassEvent
    {
        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string EventType { get; set; }

        /// <summary>
        /// Gets the event type as <see cref="KnownEventTypes"/>.
        /// </summary>
        [JsonIgnore]
        public KnownEventTypes KnownEventType => this.EventType.AsKnownEventType();

        /// <summary>
        /// Gets or sets the time at which the event was fired.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTimeOffset TimeFired { get; set; }

        /// <summary>
        /// Gets or sets the origin that fired the event.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the fired event.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public JRaw Data { get; set; }

        /// <summary>
        /// Gets or sets the context associated with the fired event.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Deserializes the event <see cref="Data"/> to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <returns>The deserialized data object.</returns>
        public T DeserializeData<T>()
        {
            try
            {
                return HassSerializer.DeserializeObject<T>(this.Data);
            }
            catch (JsonException)
            {
                throw;
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"Event {this.EventType} fired at {this.TimeFired} from {this.Origin}";
    }
}
