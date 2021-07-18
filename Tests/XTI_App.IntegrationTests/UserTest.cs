using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Configuration.Extensions;

namespace XTI_App.IntegrationTests
{
    public sealed class UserTest
    {
        [Test]
        public async Task ShouldRetrieveSystemUser()
        {
            var services = setup();
            var factory = services.GetService<AppFactory>();
            var userName = AppUserName.SystemUser(new AppKey("Authenticator", AppType.Values.WebApp), "guinevere");
            var user = await factory.Users().User(userName);
            Assert.That(user.UserName(), Is.EqualTo(userName), "Should retrieve system user");
        }

        [Test]
        public async Task ShouldAddSystemUser()
        {
            var services = setup();
            var factory = services.GetService<AppFactory>();
            var userName = AppUserName.SystemUser(new AppKey("Authenticator", AppType.Values.WebApp), "guinevere");
            var hashedPasswordFactory = new Md5HashedPasswordFactory();
            var hashedPassword = hashedPasswordFactory.Create(Guid.NewGuid().ToString("N"));
            var user = await factory.Users().Add(userName, hashedPassword, new PersonName("Test"), new EmailAddress(""), DateTimeOffset.Now);
            Assert.That(user.UserName(), Is.EqualTo(userName), "Should add system user");
        }

        private static IServiceProvider setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config) =>
                    {
                        config.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { });
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddXtiTestServices(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider;
        }

    }
}
