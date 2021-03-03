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
    public static class KnownEnumHelpers
    {
        private static Map<string, KnownDomains> knownDomainsCache = new Map<string, KnownDomains>();

        /// <summary>
        /// Converts a given <paramref name="domain"/> to <see cref="KnownDomains"/>.
        /// </summary>
        /// <param name="domain">The domain. (e.g. <c>light</c>).</param>
        /// <returns>
        /// The domain as a <see cref="KnownDomains"/> if defined; otherwise, <see cref="KnownDomains.Undefined"/>.
        /// </returns>
        public static KnownDomains AsKnownDomain(this string domain)
        {
            if (!knownDomainsCache.Forward.TryGetValue(domain, out var result) &&
                HassSerializer.TryGetEnumFromSnakeCase(domain, out result))
            {
                knownDomainsCache.Add(domain, result);
            }

            return result;
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
            if (!knownDomainsCache.Reverse.TryGetValue(domain, out var result))
            {
                result = domain.ToSnakeCaseUnchecked();
                knownDomainsCache.Add(result, domain);
            }

            return result;
        }

        private static Map<string, KnownServices> knownServicesCache = new Map<string, KnownServices>();

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
            if (!knownServicesCache.Forward.TryGetValue(service, out var result) &&
                HassSerializer.TryGetEnumFromSnakeCase(service, out result))
            {
                knownServicesCache.Add(service, result);
            }

            return result;
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
            if (!knownServicesCache.Reverse.TryGetValue(service, out var result))
            {
                result = service.ToSnakeCaseUnchecked();
                knownServicesCache.Add(result, service);
            }

            return result;
        }

        private static Map<string, KnownEventTypes> knownEventTypesCache = new Map<string, KnownEventTypes>();

        /// <summary>
        /// Converts a given snake case <paramref name="eventType"/> to <see cref="KnownDomains"/>.
        /// </summary>
        /// <param name="eventType">
        /// The event type as a snake case <see cref="string"/>. (e.g. <c>state_changed</c>).
        /// </param>
        /// <returns>
        /// The event type as a <see cref="KnownEventTypes"/> if defined; otherwise, <see cref="KnownEventTypes.Any"/>.
        /// </returns>
        public static KnownEventTypes AsKnownEventType(this string eventType)
        {
            if (!knownEventTypesCache.Forward.TryGetValue(eventType, out var result) &&
                HassSerializer.TryGetEnumFromSnakeCase(eventType, out result))
            {
                knownEventTypesCache.Add(eventType, result);
            }

            return result;
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
            if (!knownEventTypesCache.Reverse.TryGetValue(eventType, out var result))
            {
                result = eventType.ToSnakeCaseUnchecked();
                knownEventTypesCache.Add(result, eventType);
            }

            return result;
        }
    }
}
