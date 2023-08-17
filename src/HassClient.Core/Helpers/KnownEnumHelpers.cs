using HassClient.Models;
using HassClient.Serialization;
using System;

namespace HassClient.Helpers
{
    /// <summary>
    /// Contains extension methods to convert between known enums such as <see cref="KnownDomains"/>,
    /// <see cref="KnownServices"/> or <see cref="KnownEventTypes"/> to snake case strings.
    /// <para>
    /// It uses an internal cache, so these methods should be used instead of
    /// <see cref="HassSerializer.ToSnakeCase{TEnum}(TEnum)"/>.
    /// </para>
    /// </summary>
    public static partial class KnownEnumHelpers
    {
        private static KnownEnumCache<KnownDomains> knownDomainsCache = new KnownEnumCache<KnownDomains>();

        /// <summary>
        /// Converts a given <paramref name="domain"/> to <see cref="KnownDomains"/>.
        /// </summary>
        /// <param name="domain">The domain. (e.g. <c>light</c>).</param>
        /// <returns>
        /// The domain as a <see cref="KnownDomains"/> if defined; otherwise, <see cref="KnownDomains.Undefined"/>.
        /// </returns>
        public static KnownDomains AsKnownDomain(this string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException($"'{nameof(domain)}' cannot be null or empty", nameof(domain));
            }

            return knownDomainsCache.AsEnum(domain);
        }

        /// <summary>
        /// Converts a given <see cref="KnownDomains"/> to a snake case <see cref="string"/>.
        /// </summary>
        /// <param name="domain">A <see cref="KnownDomains"/>.</param>
        /// <returns>
        /// The domain as a <see cref="string"/>.
        /// </returns>
        public static string ToDomainString(this KnownDomains domain)
        {
            return knownDomainsCache.AsString(domain);
        }

        private static KnownEnumCache<KnownEventTypes> knownEventTypesCache = new KnownEnumCache<KnownEventTypes>();

        /// <summary>
        /// Converts a given snake case <paramref name="eventType"/> to <see cref="KnownEventTypes"/>.
        /// </summary>
        /// <param name="eventType">
        /// The event type as a snake case <see cref="string"/>. (e.g. <c>state_changed</c>).
        /// </param>
        /// <returns>
        /// The event type as a <see cref="KnownEventTypes"/> if defined; otherwise, <see cref="KnownEventTypes.Any"/>.
        /// </returns>
        public static KnownEventTypes AsKnownEventType(this string eventType)
        {
            if (string.IsNullOrEmpty(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or empty", nameof(eventType));
            }

            return knownEventTypesCache.AsEnum(eventType);
        }

        /// <summary>
        /// Converts a given <see cref="KnownEventTypes"/> to a snake case <see cref="string"/>.
        /// </summary>
        /// <param name="eventType">A <see cref="KnownEventTypes"/>.</param>
        /// <returns>
        /// The service as a <see cref="string"/>.
        /// </returns>
        public static string ToEventTypeString(this KnownEventTypes eventType)
        {
            return knownEventTypesCache.AsString(eventType);
        }

        private static KnownEnumCache<KnownPipelineEventTypes> knownPipelineEventTypesCache = new KnownEnumCache<KnownPipelineEventTypes>();

        /// <summary>
        /// Converts a given snake case <paramref name="eventType"/> to <see cref="KnownPipelineEventTypes"/>.
        /// </summary>
        /// <param name="eventType">
        /// The event type as a snake case <see cref="string"/>. (e.g. <c>state_changed</c>).
        /// </param>
        /// <returns>
        /// The event type as a <see cref="KnownPipelineEventTypes"/> if defined; otherwise, <see cref="KnownPipelineEventTypes.Undefined"/>.
        /// </returns>
        public static KnownPipelineEventTypes AsKnownPipelineEventType(this string eventType)
        {
            if (string.IsNullOrEmpty(eventType))
            {
                throw new ArgumentException($"'{nameof(eventType)}' cannot be null or empty", nameof(eventType));
            }

            return knownPipelineEventTypesCache.AsEnum(eventType);
        }

        /// <summary>
        /// Converts a given <see cref="KnownPipelineEventTypes"/> to a snake case <see cref="string"/>.
        /// </summary>
        /// <param name="eventType">A <see cref="KnownPipelineEventTypes"/>.</param>
        /// <returns>
        /// The service as a <see cref="string"/>.
        /// </returns>
        public static string ToEventTypeString(this KnownPipelineEventTypes eventType)
        {
            return knownPipelineEventTypesCache.AsString(eventType);
        }

        private static KnownEnumCache<KnownServices> knownServicesCache = new KnownEnumCache<KnownServices>();

        /// <summary>
        /// Converts a given snake case <paramref name="service"/> to <see cref="KnownServices"/>.
        /// </summary>
        /// <param name="service">
        /// The service as a snake case <see cref="string"/>. (e.g. <c>turn_on</c>).
        /// </param>
        /// <returns>
        /// The service as a <see cref="KnownServices"/> if defined; otherwise, <see cref="KnownServices.Undefined"/>.
        /// </returns>
        public static KnownServices AsKnownService(this string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentException($"'{nameof(service)}' cannot be null or empty", nameof(service));
            }

            return knownServicesCache.AsEnum(service);
        }

        /// <summary>
        /// Converts a given <see cref="KnownServices"/> to a snake case <see cref="string"/>.
        /// </summary>
        /// <param name="service">A <see cref="KnownServices"/>.</param>
        /// <returns>
        /// The service as a <see cref="string"/>.
        /// </returns>
        public static string ToServiceString(this KnownServices service)
        {
            return knownServicesCache.AsString(service);
        }

        private static KnownEnumCache<KnownStates> knownStatesCache = new KnownEnumCache<KnownStates>(KnownStates.Unknown);

        /// <summary>
        /// Converts a given snake case <paramref name="state"/> to <see cref="KnownStates"/>.
        /// </summary>
        /// <param name="state">
        /// The state as a snake case <see cref="string"/>. (e.g. <c>above_horizon</c>).
        /// </param>
        /// <returns>
        /// The state as a <see cref="KnownStates"/> if defined; otherwise, <see cref="KnownStates.Undefined"/>.
        /// </returns>
        public static KnownStates AsKnownState(this string state)
        {
            return knownStatesCache.AsEnum(state);
        }

        /// <summary>
        /// Converts a given <see cref="KnownStates"/> to a snake case <see cref="string"/>.
        /// </summary>
        /// <param name="state">A <see cref="KnownStates"/>.</param>
        /// <returns>
        /// The state as a <see cref="string"/>.
        /// </returns>
        public static string ToStateString(this KnownStates state)
        {
            return knownStatesCache.AsString(state);
        }
    }
}
