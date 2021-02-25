using HassClient.Entities.Decorators;
using HassClient.Models;
using HassClient.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace HassClient.Entities.Tests
{
    [TestOf(typeof(DeviceTrackerParams))]
    public class DeviceTrackerParamsTests
    {
        [Test]
        public void DefaultInstanceWithoutDeviceIdCannotBeSerialized()
        {
            var data = new DeviceTrackerParams();

            Assert.Throws<JsonSerializationException>(() => HassSerializer.SerializeObject(data));
        }

        [Test]
        public void DefaultInstanceSerializesWithDeviceIdAndLocationNameOnly()
        {
            var data = new DeviceTrackerParams() { deviceId = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasFieldsCount(2, result);
            JsonAssert.HasField("dev_id", "test", result);
            JsonAssert.HasField("location_name", result);
        }

        [Test]
        public void DefaultInstanceSerializesWithLocationNameAsNotHome()
        {
            var data = new DeviceTrackerParams() { deviceId = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("location_name", "not_home", result);
        }

        [Test]
        public void LatitudeIsClamped()
        {
            var lowerData = new DeviceTrackerParams { Latitude = -1000 };
            var upperData = new DeviceTrackerParams { Latitude = 1000 };

            Assert.AreEqual(-90, lowerData.Latitude);
            Assert.AreEqual(90, upperData.Latitude);
        }

        [Test]
        public void LongitudeIsClamped()
        {
            var lowerData = new DeviceTrackerParams { Longitude = -1000 };
            var upperData = new DeviceTrackerParams { Longitude = 1000 };

            Assert.AreEqual(-180, lowerData.Longitude);
            Assert.AreEqual(180, upperData.Longitude);
        }

        [Test]
        public void SetInvalidMacAddressThrows()
        {
            var data = new DeviceTrackerParams();

            Assert.Throws<FormatException>(() => data.MacAddress = "invalid_value");
        }

        [Test]
        public void SetNullMacAddressDowsNotThrows()
        {
            var data = new DeviceTrackerParams() { MacAddress = "AA:BB:CC:DD:EE:FF" };

            Assert.DoesNotThrow(() => data.MacAddress = null);
        }

        [Test]
        public void ValidationWithoutCoordinatesDoesNotThrows()
        {
            var data = new DeviceTrackerParams();

            Assert.DoesNotThrow(() => data.CheckValues());
        }

        [Test]
        public void ValidationWitBothCoordinatesDoesNotThrows()
        {
            var data = new DeviceTrackerParams() { Latitude = 20, Longitude = 30 };

            Assert.DoesNotThrow(() => data.CheckValues());
        }

        [Test]
        public void ValidationWithLatitudeCoordinateOnlyThrows()
        {
            var data = new DeviceTrackerParams() { Latitude = 20 };

            Assert.Throws<ArgumentException>(() => data.CheckValues());
        }

        [Test]
        public void ValidationWithLongitudeCoordinateOnlyThrows()
        {
            var data = new DeviceTrackerParams() { Longitude = 20 };

            Assert.Throws<ArgumentException>(() => data.CheckValues());
        }

        [Test]
        public void LocationIsSerialized()
        {
            var testZone = Zone.CreateUnmodified("id", "name", 20, 30, 5);
            var data = new DeviceTrackerParams { deviceId = "test", Location = testZone };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("location_name", "name", result);
        }

        [Test]
        public void HostNameIsSerialized()
        {
            var data = new DeviceTrackerParams { deviceId = "test", HostName = "testHostName" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("host_name", "testHostName", result);
        }

        [Test]
        public void GPSAccuracyIsSerialized()
        {
            var data = new DeviceTrackerParams { deviceId = "test", GPSAccuracy = 21 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("gps_accuracy", 21, result);
        }

        [Test]
        public void BatteryLevelIsSerialized()
        {
            var data = new DeviceTrackerParams { deviceId = "test", BatteryLevel = 21 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("battery", 21, result);
        }

        [Test]
        public void MacAddressIsSerialized()
        {
            var data = new DeviceTrackerParams { deviceId = "test", MacAddress = "AA:BB:CC:DD:EE:FF" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("mac", "AA:BB:CC:DD:EE:FF", result);
        }

        [Test]
        public void GPSCoordinatesAreSerialized()
        {
            var data = new DeviceTrackerParams { deviceId = "test", Latitude = 20, Longitude = 30 };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.HasField("gps", new[] { 20f, 30f }, result);
        }
        /*
        [Test]
        public void PresetModeNameIsSerialized()
        {
            var data = new DeviceTrackerParams { PresetModeName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("preset_mode", "test", result);
        }

        [Test]
        public void SpeedIsSerialized()
        {
            var data = new DeviceTrackerParams { Speed = KnownFanSpeeds.Low };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("speed", "low", result);
        }

        [Test]
        public void SpeedNameIsSerialized()
        {
            var data = new DeviceTrackerParams { SpeedName = "test" };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("speed", "test", result);
        }

        [Test]
        public void SpeedPercentageIsSerialized()
        {
            var data = new DeviceTrackerParams { SpeedPercentage = 50f };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("percentage", "50.0", result);
        }

        [Test]
        public void SpeedPercentageStepIsSerialized()
        {
            var data = new DeviceTrackerParams { SpeedPercentageStep = 50f };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsSingleField("percentage_step", "50.0", result);
        }

        [Test]
        public void UnknownPresetModeIsNotSerialized()
        {
            var data = new DeviceTrackerParams { PresetMode = KnownFanPresetModes.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }

        [Test]
        public void UnknownSpeedIsNotSerialized()
        {
            var data = new DeviceTrackerParams { Speed = KnownFanSpeeds.Unknown };
            var result = HassSerializer.SerializeObject(data);

            JsonAssert.IsEmpty(result);
        }*/
    }
}
