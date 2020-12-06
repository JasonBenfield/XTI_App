using MainDB.EF;
using MainDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.EF;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Configuration.Extensions;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.IntegrationTests
{
    public sealed class AppRoleIntegrationTest
    {
        [Test]
        public async Task ShouldAddRoleToApp()
        {
            var input = await setup();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await input.App.Role(adminRoleName);
            Assert.That(adminRole.Name(), Is.EqualTo(adminRoleName), "Should add role to app");
        }

        [Test]
        public async Task ShouldAddRoleToUser()
        {
            var input = await setup();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await input.App.Role(adminRoleName);
            var user = await input.Factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), input.Clock.Now()
            );
            await user.AddRole(adminRole);
            var userRoles = (await user.RolesForApp(input.App)).ToArray();
            Assert.That(userRoles.Length, Is.EqualTo(1), "Should add role to user");
            Assert.That(userRoles[0].IsRole(adminRole), Is.True, "Should add role to user");
        }

        [Test]
        public async Task ShouldAddRoleForDifferentAppsToUser()
        {
            var input = await setup();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await input.App.Role(adminRoleName);
            var user = await input.Factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), input.Clock.Now()
            );
            await user.AddRole(adminRole);
            var app2 = await input.Factory.Apps().Add(new AppKey("app2", AppType.Values.WebApp), "App 2", input.Clock.Now());
            var role2 = await app2.AddRole(new AppRoleName("another role"));
            await user.AddRole(role2);
            var userRoles = (await user.RolesForApp(input.App)).ToArray();
            Assert.That(userRoles.Length, Is.EqualTo(1), "Should add role to user");
            Assert.That(userRoles[0].IsRole(adminRole), Is.True, "Should add role to user");
            var userRoles2 = (await user.RolesForApp(app2)).ToArray();
            Assert.That(userRoles2.Length, Is.EqualTo(1), "Should add role to user for a different app");
            Assert.That(userRoles2[0].IsRole(role2), Is.True, "Should add role to user for a different app");
        }

        private async Task<TestInput> setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, builder) => builder.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { })
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddAppDbContextForSqlServer(hostContext.Configuration);
                        services.AddScoped<Clock, FakeClock>();
                        services.AddScoped<AppFactory, EfAppFactory>();
                        services.AddScoped<MainDbReset>();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var factory = scope.ServiceProvider.GetService<AppFactory>();
            var appDbReset = scope.ServiceProvider.GetService<MainDbReset>();
            await appDbReset.Run();
            var clock = scope.ServiceProvider.GetService<Clock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run();
            var input = new TestInput(scope.ServiceProvider, setup.App);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(IServiceProvider sp, App app)
            {
                Factory = sp.GetService<AppFactory>();
                App = app;
                Clock = sp.GetService<Clock>();
            }

            public AppFactory Factory { get; }
            public App App { get; }
            public Clock Clock { get; }
        }
    }
}
