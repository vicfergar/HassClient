using HassClient.Models;
using System;

namespace HassClient.Helpers
{
    /// <summary>
    /// Contains extension methods to operate with entities ids.
    /// </summary>
    public static class EntityIdHelpers
    {
        private const char EntitySeparator = '.';

        /// <summary>
        /// Splits the given <paramref name="entityId"/> in two parts.
        /// </summary>
        /// <param name="entityId">An entity id. (e.g. <c>light.livingroom</c>).</param>
        /// <returns>An array of two strings.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid <paramref name="entityId"/> is set.
        /// </exception>
        public static string[] SplitEntityId(this string entityId)
        {
            if (!entityId.IsValidEntityId())
            {
                throw new ArgumentException($"Invalid entity Id: {entityId}");
            }

            return entityId.Split(EntitySeparator);
        }

        /// <summary>
        /// Gets the domain of the given <paramref name="entityId"/>.
        /// </summary>
        /// <param name="entityId">An entity id. (e.g. <c>light.livingroom</c>).</param>
        /// <returns>The domain as a <see cref="string"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid <paramref name="entityId"/> is set.</exception>
        public static string GetDomain(this string entityId)
        {
            return entityId.SplitEntityId()[0];
        }

        /// <summary>
        /// Checks if two entity ids shares the same domain.
        /// </summary>
        /// <param name="entityId">An entity id. (e.g. <c>light.livingroom</c>).</param>
        /// <param name="secondEntityId">Another entity id. (e.g. <c>light.kitchen</c>).</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether both entity ids shares the same domain.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when an invalid <paramref name="entityId"/> is set.</exception>
        public static bool HasSameDomain(this string entityId, string secondEntityId)
        {
            return entityId.GetDomain() == secondEntityId.GetDomain();
        }

        /// <summary>
        /// Validates if a given entity id has a valid format.
        /// </summary>
        /// <param name="entityId">An entity id to test. (e.g. <c>light.livingroom</c>).</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether the <paramref name="entityId"/> has a valid format.
        /// </returns>
        public static bool IsValidEntityId(this string entityId)
        {
            return entityId == null ||
                   entityId.IndexOf(EntitySeparator) == entityId.LastIndexOf(EntitySeparator);
        }

        /// <summary>
        /// Validates if a given <paramref name="entityId"/> has a valid format and has specified <paramref name="domain"/>.
        /// </summary>
        /// <param name="entityId">An entity id to test. (e.g. <c>light.livingroom</c>).</param>
        /// <param name="domain">The expected domain for the <paramref name="entityId"/>.</param>
        /// <returns>
        /// A <see cref="bool"/> indicating whether the <paramref name="entityId"/> has a valid format.
        /// </returns>
        public static bool IsValidDomainEntityId(this string entityId, KnownDomains domain)
        {
            return entityId.IsValidEntityId() &&
                   entityId.GetDomain().AsKnownDomain() == domain;
        }
    }
}
