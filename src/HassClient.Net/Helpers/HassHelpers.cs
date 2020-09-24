using System;

namespace HassClient.Net.Helpers
{
    internal static class HassHelpers
    {
        public static string[] SplitEntityId(this string entityId)
        {
            if (entityId?.Contains('.') != true)
            {
                throw new ArgumentException($"Invalid entity Id: {entityId}");
            }

            return entityId.Split('.');
        }

        public static string GetDomain(this string entityId)
        {
            return entityId.SplitEntityId()[0];
        }

        public static bool HasSameDomain(this string entityId, string secondEntityId)
        {
            return entityId.GetDomain() == secondEntityId.GetDomain();
        }
    }
}
