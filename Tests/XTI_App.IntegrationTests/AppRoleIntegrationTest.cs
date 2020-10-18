using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.DB;
using XTI_App.EF;
using XTI_App.Extensions;
using XTI_App.Fakes;
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
            await input.App.AddRole(adminRoleName);
            var roles = (await input.App.Roles()).ToArray();
            Assert.That(roles.Length, Is.EqualTo(1), "Should add role to app");
            Assert.That(roles[0].Name(), Is.EqualTo(adminRoleName), "Should add role to app");
        }

        [Test]
        public async Task ShouldAddRoleToUser()
        {
            var input = await setup();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await input.App.AddRole(adminRoleName);
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
            var adminRole = await input.App.AddRole(adminRoleName);
            var user = await input.Factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), input.Clock.Now()
            );
            await user.AddRole(adminRole);
            var app2 = await input.Factory.Apps().AddApp(new AppKey("app2"), AppType.Values.WebApp, "App 2", input.Clock.Now());
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
            var hostEnv = new FakeHostEnvironment { EnvironmentName = "Test" };
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", hostEnv.EnvironmentName);
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.UseXtiConfiguration("Test", new string[] { });
            var configuration = configurationBuilder.Build();
            services.AddScoped<IHostEnvironment>(sp => hostEnv);
            services.AddAppDbContextForSqlServer(configuration);
            services.AddScoped<Clock, FakeClock>();
            services.AddScoped<AppFactory, EfAppFactory>();
            services.AddScoped<AppDbReset>();
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<AppFactory>();
            var appDbReset = sp.GetService<AppDbReset>();
            await appDbReset.Run();
            await new AppSetup(factory).Run();
            var app = await factory.Apps().AddApp(new AppKey("Fake"), AppType.Values.WebApp, "Fake", DateTime.UtcNow);
            var input = new TestInput(sp, app);
            return input;
        }

        private sealed class TestInput
        {
            public TestInput(ServiceProvider sp, App app)
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
