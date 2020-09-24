using HassClient.Net.ClientWebSocket;
using HassClient.Net.Serialization;
using HassClient.Net.Tests.Mocks.HassServer;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HassClient.Net.Tests
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
                this.hassServer.Start();
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