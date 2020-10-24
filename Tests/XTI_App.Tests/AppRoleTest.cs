using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.EF;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Core;
using XTI_Core.Fakes;

namespace XTI_App.Tests
{
    public sealed class AppRoleTest
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
        public async Task ShouldNotAddRoleFromAppRoleNames_WhenTheRoleAlreadyExists()
        {
            var input = await setup();
            var roleNames = FakeAppRoles.Instance.Values();
            await input.App.SetRoles(roleNames);
            await input.App.SetRoles(roleNames);
            var appRoles = await input.App.Roles();
            Assert.That(appRoles.Select(r => r.Name()), Is.EquivalentTo(roleNames), "Should add role names from app role names");
        }

        [Test]
        public async Task ShouldRemoveRolesNotInAppRoleNames()
        {
            var input = await setup();
            var roleNames = FakeAppRoles.Instance.Values();
            await input.App.SetRoles(roleNames);
            roleNames = roleNames.Where(rn => !rn.Equals(FakeAppRoles.Instance.Manager)).ToArray();
            await input.App.SetRoles(roleNames);
            var appRoles = await input.App.Roles();
            Assert.That(appRoles.Select(r => r.Name()), Is.EquivalentTo(roleNames), "Should add role names from app role names");
        }

        private async Task<TestInput> setup()
        {
            var services = new ServiceCollection();
            services.AddServicesForTests();
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<FakeClock>();
            var setup = new FakeAppSetup(factory, clock);
            await setup.Run();
            var app = await factory.Apps().Add(new AppKey("Fake", AppType.Values.WebApp), "Fake", clock.Now());
            return new TestInput(sp, app);
        }

        private sealed class TestInput
        {
            public TestInput(ServiceProvider sp, App app)
            {
                Factory = sp.GetService<AppFactory>();
                Clock = sp.GetService<FakeClock>();
                App = app;
            }

            public AppFactory Factory { get; }
            public FakeClock Clock { get; }
            public App App { get; }
        }
    }
}
