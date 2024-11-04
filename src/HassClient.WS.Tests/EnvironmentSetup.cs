using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;
using HassClient.WS.Tests.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace HassClient.WS.Tests
{
    [SetUpFixture]
    public class EnvironmentSetup
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private IContainer hassContainer;

        [OneTimeSetUp]
        public async Task GlobalSetupAsync()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWSApiTest.TestsInstanceBaseUrlVar);

            if (instanceBaseUrl == null)
            {
                // Create temporary directory with tests resources
                var tmpDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tmpDirectory);
                DirectoryExtensions.CopyFilesRecursively("./resources", tmpDirectory);

                const int HassPort = 8123;
                const string HassVersion = "latest";
                const string tokenFilename = "TOKEN";
                var testcontainersBuilder = new ContainerBuilder()
                    .WithImage($"homeassistant/home-assistant:{HassVersion}")
                    .WithPortBinding(HassPort, assignRandomHostPort: true)
                    .WithExposedPort(HassPort)
                    .WithBindMount(Path.Combine(tmpDirectory, "config"), "/config")
                    .WithBindMount(Path.Combine(tmpDirectory, "scripts"), "/app")
                    .WithWaitStrategy(Wait.ForUnixContainer()
                                            .UntilPortIsAvailable(HassPort))
                    .WithEntrypoint("/bin/sh", "-c")
                    .WithCommand($"python3 /app/create_token.py >/app/{tokenFilename} && /init");

                this.hassContainer = testcontainersBuilder.Build();
                await this.hassContainer.StartAsync();

                var mappedPort = this.hassContainer.GetMappedPublicPort(HassPort);
                var hostTokenPath = Path.Combine(tmpDirectory, "scripts", tokenFilename);
                var accessToken = File.ReadLines(hostTokenPath).First();

                Environment.SetEnvironmentVariable(BaseHassWSApiTest.TestsInstanceBaseUrlVar, $"http://localhost:{mappedPort}");
                Environment.SetEnvironmentVariable(BaseHassWSApiTest.TestsAccessTokenVar, accessToken);
            }

            await this.EnsureHassIsRunningAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            this.cts.Cancel();
            this.cts.Dispose();

            if (this.hassContainer != null)
            {
                await this.hassContainer.DisposeAsync();
            }
        }

        private async Task EnsureHassIsRunningAsync()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWSApiTest.TestsInstanceBaseUrlVar);
            var accessToken = Environment.GetEnvironmentVariable(BaseHassWSApiTest.TestsAccessTokenVar);
            var connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(instanceBaseUrl, accessToken);

            var cancellationToken = this.cts.Token;
            var hassWSApi = new HassWSApi();
            await hassWSApi.ConnectAsync(connectionParameters, cancellationToken: cancellationToken);

            const int maxRetries = 3;
            const int delayMilliseconds = 1000;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                var config = await hassWSApi.GetConfigurationAsync(cancellationToken);
                if (config?.State == "RUNNING")
                {
                    return;
                }

                if (attempt < maxRetries - 1)
                {
                    await Task.Delay(delayMilliseconds, cancellationToken);
                }
            }

            Assert.Fail("Home Assistant is not running after multiple attempts");
        }
    }
}
