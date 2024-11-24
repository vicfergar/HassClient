using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands.Subscriptions;
using HassClient.WS.Tests.Mocks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class PushNotificationsTests : BaseHassWSApiTest
    {
        private async Task<string> RegisterMobileAppAsync()
        {
            var baseUrl = this.InstanceBaseUrl;
            var accessToken = this.AccessToken;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var registrationData = new
            {
                device_id = "my_test_device",
                app_id = "my_test_app",
                app_name = "Test App",
                app_version = "1.0",
                device_name = "Test Device",
                manufacturer = "Test",
                model = "Test",
                os_name = "Linux",
                os_version = "1.0",
                supports_encryption = false,
                app_data = new
                {
                    push_websocket_channel = true
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(registrationData),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"{baseUrl}/api/mobile_app/registrations",
                content);

            response.EnsureSuccessStatusCode();
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            return result["webhook_id"].ToString();
        }

        [Test]
        public async Task PushNotificationTest()
        {
            var webhookId = await this.RegisterMobileAppAsync();

            var listener = new MockEventListener();
            var registerResult = await this.hassWSApi.RegisterPushNotificationChannelAsync(webhookId, supportConfirm: true, listener.Handle);

            Assert.IsTrue(registerResult, "Subscription should succeed");

            var notifyResult = await this.hassWSApi.Services.CallAsync("notify", "mobile_app_test_device", new
            {
                message = "Test message",
                title = "Test title",
                target="mobile_app.my_test_device",
                data = new
                {
                    string_key = "data_value",
                    bool_key = true,
                    int_key = 1337
                }
            });
            Assert.NotNull(notifyResult);

            var eventData = await listener.WaitFirstEventWithTimeoutAsync<IncomingEventMessage>(millisecondsTimeout: 500);
            Assert.NotZero(listener.HitCount);
            Assert.NotNull(eventData);
            var notification = eventData.Args.DeserializeEvent<PushNotification>();
            Assert.AreEqual("Test message", notification.Message);
            Assert.AreEqual("Test title", notification.Title);
            Assert.NotNull(notification.Data);
            Assert.AreEqual("data_value", notification.Data["string_key"]);
            Assert.AreEqual(true, notification.Data["bool_key"]);
            Assert.AreEqual(1337, notification.Data["int_key"]);
            Assert.NotNull(notification.HassConfirmId);

            var confirmSuccess = await this.hassWSApi.ConfirmPushNotificationAsync(webhookId, notification.HassConfirmId);
            Assert.IsTrue(confirmSuccess);
        }
    }
}
