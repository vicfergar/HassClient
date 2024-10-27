using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;
using HassClient.WS.Tests.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [SetUpFixture]
    public class EnvironmentSetup
    {
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
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (this.hassContainer != null)
            {
                await this.hassContainer.DisposeAsync();
            }
        }
    }
}
