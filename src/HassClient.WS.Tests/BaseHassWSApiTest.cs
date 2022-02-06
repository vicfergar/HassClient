using HassClient.Serialization;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public abstract class BaseHassWSApiTest
    {
        private const string TestsInstanceBaseUrlVar = "TestsInstanceBaseUrl";
        private const string TestsAccessTokenVar = "TestsAccessToken";

        private readonly ConnectionParameters connectionParameters;

        protected HassWSApi hassWSApi;

        public BaseHassWSApiTest()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(TestsInstanceBaseUrlVar);
            var accessToken = Environment.GetEnvironmentVariable(TestsAccessTokenVar);

            if (instanceBaseUrl == null)
            {
                Assert.Ignore($"Hass instance base URL for tests not provided. It should be set in the environment variable '{TestsInstanceBaseUrlVar}'");
            }

            if (accessToken == null)
            {
                Assert.Ignore($"Hass access token for tests not provided. It should be set in the environment variable '{TestsAccessTokenVar}'");
            }

            this.connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(instanceBaseUrl, accessToken);
        }

        [OneTimeSetUp]
        protected virtual async Task OneTimeSetUp()
        {
            this.hassWSApi = new HassWSApi();
            await this.hassWSApi.ConnectAsync(this.connectionParameters);

            HassSerializer.DefaultSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            HassSerializer.DefaultSettings.Error += this.HassSerializerError;

            Assert.AreEqual(this.hassWSApi.ConnectionState, ConnectionStates.Connected, "SetUp failed");
        }

        private void HassSerializerError(object sender, ErrorEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.ErrorContext.Path))
            {
                args.ErrorContext.Handled = true;
                Assert.Fail(args.ErrorContext.Error.Message);
            }
        }

        [OneTimeTearDown]
        protected virtual Task OneTimeTearDown()
        {
            return Task.CompletedTask;
        }
    }
}