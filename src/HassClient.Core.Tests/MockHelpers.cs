using HassClient.Helpers;
using HassClient.Models;
using System;
using System.Runtime.CompilerServices;

namespace HassClient.Core.Tests
{
    public static class MockHelpers
    {
        public static string GetRandomEntityId(KnownDomains domain)
        {
            return $"{domain.ToDomainString()}.{DateTime.Now.Ticks}";
        }

        public static string GetRandomTestName([CallerMemberName] string prefix = null)
        {
            return $"{prefix}_{DateTime.Now.Ticks}";
        }
    }
}
