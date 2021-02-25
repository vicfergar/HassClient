using HassClient.Models;
using HassClient.Entities.Decorators;
using HassClient.Serialization;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace HassClient.Entities.Tests
{
    [TestOf(typeof(LightParams))]
    public class LightParamsTests
    {
        [Test]
        public void DefaultInstanceSerializesAsEmpty()
        {
            var data = new LightParams();
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }

        [Test]
        public void TransitionIsClamped()
        {
            var data = new LightParams { Transition = 1000 };

            Assert.AreEqual(300, data.Transition);
        }

        [Test]
        public void BrightnessStepIsClamped()
        {
            var lowerData = new LightParams { BrightnessStep = -1000 };
            var upperData = new LightParams { BrightnessStep = 1000 };

            Assert.AreEqual(-100, lowerData.BrightnessStep);
            Assert.AreEqual(100, upperData.BrightnessStep);
        }

        [Test]
        public void BrightnessStepValueIsClamped()
        {
            var lowerData = new LightParams { BrightnessStepValue = -1000 };
            var upperData = new LightParams { BrightnessStepValue = 1000 };

            Assert.AreEqual(-255, lowerData.BrightnessStepValue);
            Assert.AreEqual(255, upperData.BrightnessStepValue);
        }

        [Test]
        public void ProfileSetsProfileName()
        {
            var data = new LightParams { Profile = KnownLightProfiles.Energize };

            Assert.AreEqual("energize", data.ProfileName);
        }

        [Test]
        public void EffectSetsEffectName()
        {
            var data = new LightParams { Effect = KnownLightEffects.Rainbow };

            Assert.AreEqual("rainbow", data.EffectName);
        }

        [Test]
        public void UnknownProfileDoesNotProfileName()
        {
            var data = new LightParams { Profile = KnownLightProfiles.Unknown };

            Assert.IsNull(data.ProfileName);
        }

        [Test]
        public void UnknownEffectDoesNotEffectName()
        {
            var data = new LightParams { Effect = KnownLightEffects.Unknown };

            Assert.IsNull(data.EffectName);
        }

        [Test]
        public void TransitionIsSerialized()
        {
            var data = new LightParams { Transition = 100 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("transition", 100, result);
        }

        [Test]
        public void NameColorIsSerialized()
        {
            var data = new LightParams { Color = NameColor.HomeAssistant };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("color_name", "homeassistant", result);
        }

        [Test]
        public void RGBColorIsSerialized()
        {
            var data = new LightParams { Color = Color.FromRGB(11, 22, 33) };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("rgb_color", new JArray(11, 22, 33), result);
        }

        [Test]
        public void HSColorIsSerialized()
        {
            var data = new LightParams { Color = Color.FromHS(11, 22) };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("hs_color", new JArray(11, 22), result);
        }

        [Test]
        public void MiredsTemperatureColorIsSerialized()
        {
            var data = new LightParams { Color = Color.FromMireds(200) };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("color_temp", 200, result);
        }

        [Test]
        public void KelvinTemperatureColorIsSerialized()
        {
            var data = new LightParams { Color = Color.FromKelvinTemperature(5000) };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("kelvin", 5000, result);
        }

        [Test]
        public void XYColorIsSerialized()
        {
            var data = new LightParams { Color = Color.FromXY(0.3f, 0.1f) };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("xy_color", new JArray(0.3, 0.1), result);
        }

        [Test]
        public void FlashModeIsSerialized()
        {
            var data = new LightParams { Flash = FlashMode.Short };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("flash", "short", result);
        }

        [Test]
        public void WhiteLevelIsSerialized()
        {
            var data = new LightParams { WhiteLevel = 200 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("white_value", 200, result);
        }

        [Test]
        public void BrightnessIsSerialized()
        {
            var data = new LightParams { Brightness = 50 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("brightness_pct", 50, result);
        }

        [Test]
        public void BrightnessValueIsSerialized()
        {
            var data = new LightParams { BrightnessValue = 200 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("brightness", 200, result);
        }

        [Test]
        public void BrightnessStepIsSerialized()
        {
            var data = new LightParams { BrightnessStep = -50 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("brightness_step_pct", -50, result);
        }

        [Test]
        public void BrightnessStepValueIsSerialized()
        {
            var data = new LightParams { BrightnessStepValue = -200 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("brightness_step", -200, result);
        }

        [Test]
        public void ProfileNameIsSerialized()
        {
            var data = new LightParams { ProfileName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("profile", "test", result);
        }

        [Test]
        public void ProfileIsSerialized()
        {
            var data = new LightParams { Profile = KnownLightProfiles.Energize };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("profile", "energize", result);
        }

        [Test]
        public void EffectNameIsSerialized()
        {
            var data = new LightParams { EffectName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("effect", "test", result);
        }

        [Test]
        public void EffectIsSerialized()
        {
            var data = new LightParams { Effect = KnownLightEffects.Rainbow };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("effect", "rainbow", result);
        }

        [Test]
        public void UnknownLightProfileIsNotSerialized()
        {
            var data = new LightParams { Profile = KnownLightProfiles.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }

        [Test]
        public void UnknownLightEffectIsNotSerialized()
        {
            var data = new LightParams { Effect = KnownLightEffects.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }
    }
}
