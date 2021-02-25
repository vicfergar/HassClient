using HassClient.Entities.Decorators;
using HassClient.Helpers;
using NUnit.Framework;

namespace HassClient.Entities.Tests
{
    [TestFixture]
    class KnownEnumTests
    {
        [Test]
        [TestCase("gps")]
        [TestCase("router")]
        [TestCase("bluetooth")]
        [TestCase("bluetooth_le")]
        public void AllKnownDeviceTrackedSourcesCanBeParsed(string snakeCaseValue)
        {
            var cache =  new KnownEnumCache<KnownDeviceTrackedSources>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(KnownDeviceTrackedSources.Unknown, result);
        }

        [Test]
        [TestCase("none")]
        [TestCase("auto")]
        [TestCase("smart")]
        [TestCase("whoosh")]
        [TestCase("eco")]
        [TestCase("breeze")]
        [TestCase("sleep")]
        [TestCase("on")]
        public void AllKnownFanPresetModesCanBeParsed(string snakeCaseValue)
        {
            var cache = new KnownEnumCache<KnownFanPresetModes>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(KnownFanPresetModes.Unknown, result);
        }

        [Test]
        [TestCase("none")]
        [TestCase("off")]
        [TestCase("low")]
        [TestCase("medium")]
        [TestCase("high")]
        public void AllKnownFanSpeedsCanBeParsed(string snakeCaseValue)
        {
            var cache = new KnownEnumCache<KnownFanSpeeds>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(KnownFanSpeeds.Unknown, result);
        }

        [Test]
        [TestCase("onoff")]
        [TestCase("brightness")]
        [TestCase("color_temp")]
        [TestCase("hs")]
        [TestCase("xy")]
        [TestCase("rgb")]
        [TestCase("rgbw")]
        [TestCase("rgbww")]
        [TestCase("white")]
        public void AllLightColorModesCanBeParsed(string snakeCaseValue)
        {
            var cache = new KnownEnumCache<LightColorModes>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(LightColorModes.Unknown, result);
        }

        [Test]
        [TestCase("none")]
        [TestCase("color_loop")]
        [TestCase("rainbow")]
        [TestCase("random")]
        public void AllKnownLightEffectsCanBeParsed(string snakeCaseValue)
        {
            var cache = new KnownEnumCache<KnownLightEffects>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(KnownLightEffects.Unknown, result);
        }

        [Test]
        [TestCase("relax")]
        [TestCase("concentrate")]
        [TestCase("energize")]
        [TestCase("reading")]
        public void AllKnownLightProfilesCanBeParsed(string snakeCaseValue)
        {
            var cache = new KnownEnumCache<KnownLightProfiles>();
            var result = cache.AsEnum(snakeCaseValue);
            Assert.AreNotEqual(KnownLightProfiles.Unknown, result);
        }
    }
}
