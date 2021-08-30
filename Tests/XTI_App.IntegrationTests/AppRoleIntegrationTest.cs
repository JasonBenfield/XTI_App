using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Fakes;
using XTI_Configuration.Extensions;
using XTI_Core;

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
        public async Task ShouldGetRoleByID()
        {
            var input = await setup();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await input.App.Role(adminRoleName);
            var role = await input.App.Role(adminRole.ID.Value);
            Assert.That(role.ID.IsValid, Is.True, "Should get role by ID");
            Assert.That(adminRole.ID, Is.EqualTo(role.ID), "Should get role by ID");
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
            var defaultModifier = await input.App.DefaultModifier();
            var userRoles = (await user.AssignedRoles(defaultModifier)).ToArray();
            Assert.That(userRoles.Length, Is.EqualTo(1), "Should add role to user");
            Assert.That(userRoles[0].ID, Is.EqualTo(adminRole.ID), "Should add role to user");
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
            var defaultModifier1 = await input.App.DefaultModifier();
            var userRoles = (await user.AssignedRoles(defaultModifier1)).ToArray();
            Assert.That(userRoles.Length, Is.EqualTo(1), "Should add role to user");
            Assert.That(userRoles[0].ID, Is.EqualTo(adminRole.ID), "Should add role to user");
            var defaultModCategory2 = await app2.TryAddModCategory(ModifierCategoryName.Default);
            var defaultModifier2 = await defaultModCategory2.TryAddDefaultModifier();
            var userRoles2 = (await user.AssignedRoles(defaultModifier2)).ToArray();
            Assert.That(userRoles2.Length, Is.EqualTo(1), "Should add role to user for a different app");
            Assert.That(userRoles2[0].ID, Is.EqualTo(role2.ID), "Should add role to user for a different app");
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
                        services.AddXtiTestServices(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            await scope.ServiceProvider.Reset();
            var app = await scope.ServiceProvider.FakeApp();
            var input = new TestInput(scope.ServiceProvider, app);
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
