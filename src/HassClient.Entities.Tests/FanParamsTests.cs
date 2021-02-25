using HassClient.Entities.Decorators;
using HassClient.Serialization;
using NUnit.Framework;

namespace HassClient.Entities.Tests
{
    [TestOf(typeof(FanParams))]
    public class FanParamsTests
    {
        [Test]
        public void DefaultInstanceSerializesAsEmpty()
        {
            var data = new FanParams();
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }

        [Test]
        public void SpeedPercentageIsClamped()
        {
            var lowerData = new FanParams { SpeedPercentage = -1000 };
            var upperData = new FanParams { SpeedPercentage = 1000 };

            Assert.AreEqual(0, lowerData.SpeedPercentage);
            Assert.AreEqual(100, upperData.SpeedPercentage);
        }

        [Test]
        public void SpeedPercentageStepIsClamped()
        {
            var lowerData = new FanParams { SpeedPercentageStep = -1000 };
            var upperData = new FanParams { SpeedPercentageStep = 1000 };

            Assert.AreEqual(0, lowerData.SpeedPercentageStep);
            Assert.AreEqual(100, upperData.SpeedPercentageStep);
        }

        [Test]
        public void PresetModeSetsPresetModeName()
        {
            var data = new FanParams { PresetMode = KnownFanPresetModes.Sleep };

            Assert.AreEqual("sleep", data.PresetModeName);
        }

        [Test]
        public void SpeedSetsSpeedName()
        {
            var data = new FanParams { Speed = KnownFanSpeeds.Low };

            Assert.AreEqual("low", data.SpeedName);
        }

        [Test]
        public void UnknownPresetModeDoesNotSetPresetModeName()
        {
            var data = new FanParams { PresetMode = KnownFanPresetModes.Unknown };

            Assert.IsNull(data.PresetModeName);
        }

        [Test]
        public void UnknownSpeedDoesNotSetSpeedName()
        {
            var data = new FanParams { Speed = KnownFanSpeeds.Unknown };

            Assert.IsNull(data.SpeedName);
        }

        [Test]
        public void DirectionIsSerialized()
        {
            var data = new FanParams { Direction = FanDirections.Forward };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("direction", "forward", result);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void OscillatingIsSerialized(bool value)
        {
            var data = new FanParams { Oscillating = value };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("oscillating", value, result);
        }

        [Test]
        public void PresetModeIsSerialized()
        {
            var data = new FanParams { PresetMode = KnownFanPresetModes.Sleep };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("preset_mode", "sleep", result);
        }

        [Test]
        public void PresetModeNameIsSerialized()
        {
            var data = new FanParams { PresetModeName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("preset_mode", "test", result);
        }

        [Test]
        public void SpeedIsSerialized()
        {
            var data = new FanParams { Speed = KnownFanSpeeds.Low };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("speed", "low", result);
        }

        [Test]
        public void SpeedNameIsSerialized()
        {
            var data = new FanParams { SpeedName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("speed", "test", result);
        }

        [Test]
        public void SpeedPercentageIsSerialized()
        {
            var data = new FanParams { SpeedPercentage = 50f };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("percentage", 50f, result);
        }

        [Test]
        public void SpeedPercentageStepIsSerialized()
        {
            var data = new FanParams { SpeedPercentageStep = 50f };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("percentage_step", 50f, result);
        }

        [Test]
        public void UnknownPresetModeIsNotSerialized()
        {
            var data = new FanParams { PresetMode = KnownFanPresetModes.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }

        [Test]
        public void UnknownSpeedIsNotSerialized()
        {
            var data = new FanParams { Speed = KnownFanSpeeds.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }
    }
}
