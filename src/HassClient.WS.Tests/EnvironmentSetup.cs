using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
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
        private TestcontainersContainer hassContainer;

        [OneTimeSetUp]
        public async Task GlobalSetupAsync()
        {
            var instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWSApiTest.TestsInstanceBaseUrlVar);

            if (instanceBaseUrl == null)
            {
                // Create temporary directory
                var tmpDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tmpDirectory);

                // Copy create_token script
                const string createTokenScriptPath = "./resources/scripts/create_token.py";
                var createTokenScriptFilename = Path.GetFileName(createTokenScriptPath);
                File.Copy(Path.GetFullPath(createTokenScriptPath), Path.Combine(tmpDirectory, createTokenScriptFilename));

                const int HassPort = 8123;
                const string HassVersion = "latest";
                const string hassConfigPath = "./resources/config";
                const string tokenFilename = "TOKEN";
                var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                      .WithImage($"homeassistant/home-assistant:{HassVersion}")
                      .WithPortBinding(HassPort, assignRandomHostPort: true)
                      .WithExposedPort(HassPort)
                      .WithBindMount(Path.GetFullPath(hassConfigPath), "/config")
                      .WithBindMount(tmpDirectory, "/tmp")
                      .WithWaitStrategy(Wait.ForUnixContainer()
                                            .UntilPortIsAvailable(HassPort))
                      .WithEntrypoint("/bin/bash", "-c")
                      .WithCommand($"python3 /tmp/{createTokenScriptFilename} >/tmp/{tokenFilename} && /init");

                this.hassContainer = testcontainersBuilder.Build();
                await this.hassContainer.StartAsync();

                var mappedPort = this.hassContainer.GetMappedPublicPort(HassPort);
                var hostTokenPath = Path.Combine(tmpDirectory, tokenFilename);
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
