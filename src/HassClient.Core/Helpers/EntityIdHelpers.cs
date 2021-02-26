using System;

namespace HassClient.Helpers
{
    /// <summary>
    /// Contains extension methods to operate with entities ids.
    /// </summary>
    public static class EntityIdHelpers
    {
        /// <summary>
        /// Splits the given <paramref name="entityId"/> in two parts.
        /// </summary>
        /// <param name="entityId">An entity id. (e.g. <c>light.livingroom</c>).</param>
        /// <returns>An array of two strings.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid entityId is set.</exception>
        public static string[] SplitEntityId(this string entityId)
        {
            if (entityId?.Contains('.') != true)
            {
                throw new ArgumentException($"Invalid entity Id: {entityId}");
            }

            return entityId.Split('.');
        }

        /// <summary>
        /// Gets the domain of the given <paramref name="entityId"/>.
        /// </summary>
        /// <param name="entityId">An entity id. (e.g. <c>light.livingroom</c>).</param>
        /// <returns>The domain as a <see cref="string"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid entityId is set.</exception>
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
        /// <exception cref="ArgumentException">Thrown when an invalid entityId is set.</exception>
        public static bool HasSameDomain(this string entityId, string secondEntityId)
        {
            return entityId.GetDomain() == secondEntityId.GetDomain();
        }
    }
}
