using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Core.Fakes;
using XTI_Credentials;
using XTI_Secrets;
using XTI_Secrets.Extensions;

namespace XTI_WebApp.IntegrationTests
{
    public class SecretCredentialsIntegrationTest
    {
        [Test]
        public async Task ShouldStoreAndRetrieveCredentials()
        {
            var input = setup();
            var secretCredentials = input.Factory.Create("Test");
            var storedCredentials = new CredentialValue("Someone", "Password");
            await secretCredentials.Update(storedCredentials);
            var retrievedCredentials = await secretCredentials.Value();
            Assert.That(retrievedCredentials, Is.EqualTo(storedCredentials), "Should store and retrieve credentials");
        }

        private TestInput setup()
        {
            var hostEnv = new FakeHostEnvironment { EnvironmentName = "Test" };
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", hostEnv.EnvironmentName);
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.UseXtiConfiguration("Test", new string[] { });
            var configuration = configurationBuilder.Build();
            services.AddScoped<IHostEnvironment>(sp => hostEnv);
            services.AddDataProtection(configuration);
            services.AddFileSecretCredentials();
            var sp = services.BuildServiceProvider();
            var input = new TestInput(sp);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(ServiceProvider sp)
            {
                Factory = sp.GetService<SecretCredentialsFactory>();
            }

            public SecretCredentialsFactory Factory { get; }
        }
    }
}
