using HassClient.Serialization;
using HassClient.WS.Tests.Mocks.HassServer;
using HassClient.WS;
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

        private readonly string instanceBaseUrl;

        private readonly string accessToken;

        protected MockHassServerWebSocket hassServer;

        protected HassWSApi hassWSApi;

        public BaseHassWSApiTest(bool useFakeHassServer = false)
        {
            if (useFakeHassServer)
            {
                this.hassServer = new MockHassServerWebSocket();
                this.instanceBaseUrl = this.hassServer.InstanceBaseUrl;
                this.accessToken = this.hassServer.AccessToken;
            }
            else
            {
                this.instanceBaseUrl = Environment.GetEnvironmentVariable(TestsInstanceBaseUrlVar);
                this.accessToken = Environment.GetEnvironmentVariable(TestsAccessTokenVar);

                if (this.instanceBaseUrl == null)
                {
                    Assert.Ignore($"Hass instance base URL for tests not provided. It should be set in the environment variable '{TestsInstanceBaseUrlVar}'");
                }
                else if (this.accessToken == null)
                {
                    Assert.Ignore($"Hass access token for tests not provided. It should be set in the environment variable '{TestsAccessTokenVar}'");
                }
            }
        }

        [OneTimeSetUp]
        protected virtual async Task OneTimeSetUp()
        {
            if (this.hassServer != null)
            {
                await this.hassServer.StartAsync();
            }

            this.hassWSApi = new HassWSApi();
            await this.hassWSApi.ConnectAsClientAsync(this.instanceBaseUrl, this.accessToken);

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