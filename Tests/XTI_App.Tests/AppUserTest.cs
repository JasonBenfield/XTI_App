using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Core;

namespace XTI_App.Tests
{
    public sealed class AppUserTest
    {
        [Test]
        public async Task ShouldAddUser()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            await addUser(services, userName);
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(userName);
            var userModel = user.ToModel();
            Assert.That(user.ID.IsValid(), Is.True);
            Assert.That(userModel.ID, Is.EqualTo(user.ID.Value));
            Assert.That(userModel.UserName, Is.EqualTo("test.user"));
            Assert.That(userModel.Name, Is.EqualTo("Test User"));
            Assert.That(userModel.Email, Is.EqualTo("test.user@hotmail.com"));
        }

        [Test]
        public async Task ShouldGetUsers()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            await addUser(services, userName);
            var factory = services.GetService<AppFactory>();
            var users = (await factory.Users().Users()).ToArray();
            Assert.That(users.Select(u => u.UserName()), Has.One.EqualTo(userName), "Should get all users");
        }

        [Test]
        public async Task ShouldEditUser()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            await addUser(services, userName);
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(userName);
            await user.Edit(new PersonName("Changed Name"), new EmailAddress("changed@gmail.com"));
            user = await factory.Users().User(userName);
            var userModel = user.ToModel();
            Assert.That(userModel.Name, Is.EqualTo("Changed Name"), "Should update user");
            Assert.That(userModel.Email, Is.EqualTo("changed@gmail.com"), "Should update user");
        }

        [Test]
        public async Task ShouldGetAssignedRoles()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            var user = await addUser(services, userName);
            var app = await services.FakeApp();
            var appRoles = await app.Roles();
            await user.AddRole(appRoles.First(ar => ar.Name().Equals(FakeInfo.Roles.Admin)));
            await user.AddRole(appRoles.First(ar => ar.Name().Equals(FakeInfo.Roles.Manager)));
            var userRoles = await user.AssignedRoles(app);
            Assert.That
            (
                userRoles.Select(ur => new AppRoleName(ur.Role.Name)),
                Is.EquivalentTo(new[] { FakeInfo.Roles.Admin, FakeInfo.Roles.Manager }),
                "Should get assigned roles"
            );
        }

        [Test]
        public async Task ShouldGetUnassignedRoles()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            var user = await addUser(services, userName);
            var app = await services.FakeApp();
            var appRoles = await app.Roles();
            await user.AddRole(appRoles.First(ar => ar.Name().Equals(FakeInfo.Roles.Admin)));
            await user.AddRole(appRoles.First(ar => ar.Name().Equals(FakeInfo.Roles.Manager)));
            var unassignedRoles = await user.UnassignedRoles(app);
            Assert.That
            (
                unassignedRoles.Select(r => r.Name()),
                Is.EquivalentTo(new[] { FakeInfo.Roles.Viewer }),
                "Should get unassigned roles"
            );
        }

        [Test]
        public async Task ShouldGetUserRoleByID()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            var user = await addUser(services, userName);
            var app = await services.FakeApp();
            var adminRole = await app.Role(FakeInfo.Roles.Admin);
            var addedUserRole = await user.AddRole(adminRole);
            var userRole = await app.UserRole(addedUserRole.ID.Value);
            Assert.That(userRole.IsRole(adminRole), Is.True, "Should get user role by ID");
        }

        private static async Task<AppUser> addUser(IServiceProvider services, AppUserName userName)
        {
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<Clock>();
            var user = await factory.Users().Add
            (
                userName,
                new FakeHashedPassword("Password12345"),
                new PersonName("Test User"),
                new EmailAddress("test.user@hotmail.com"),
                clock.Now()
            );
            return user;
        }

        private async Task<IServiceProvider> setup()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests();
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await sp.Setup();
            return sp;
        }

    }
}
