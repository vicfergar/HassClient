using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using HassClient.WS.Tests.Extensions;
using NUnit.Framework;
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
            string instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWSApiTest.TestsInstanceBaseUrlVar);

            if (instanceBaseUrl == null)
            {
                // Create temporary directory with tests resources
                string tmpDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tmpDirectory);
                DirectoryExtensions.CopyFilesRecursively("./resources", tmpDirectory);

                const int HassPort = 8123;
                const string HassVersion = "latest";
                const string tokenFilename = "TOKEN";
                ContainerBuilder testcontainersBuilder = new ContainerBuilder()
                      .WithImage($"homeassistant/home-assistant:{HassVersion}")
                      .WithPortBinding(HassPort, assignRandomHostPort: true)
                      .WithExposedPort(HassPort)
                      .WithBindMount(Path.Combine(tmpDirectory, "config"), "/config")
                      .WithBindMount(Path.Combine(tmpDirectory, "scripts"), "/app")
                      .WithWaitStrategy(Wait.ForUnixContainer()
                                            .UntilPortIsAvailable(HassPort))
                      .WithEntrypoint("/bin/bash", "-c")
                      .WithCommand($"python3 /app/create_token.py >/app/{tokenFilename} && /init");

                this.hassContainer = testcontainersBuilder.Build();
                await this.hassContainer.StartAsync();

                ushort mappedPort = this.hassContainer.GetMappedPublicPort(HassPort);
                string hostTokenPath = Path.Combine(tmpDirectory, "scripts", tokenFilename);
                string accessToken = File.ReadLines(hostTokenPath).First();

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
