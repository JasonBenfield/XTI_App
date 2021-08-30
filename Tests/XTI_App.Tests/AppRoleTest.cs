﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Fakes;
using XTI_App.TestFakes;
using XTI_Core;

namespace XTI_App.Tests
{
    public sealed class AppRoleTest
    {
        [Test]
        public async Task ShouldAddRoleToApp()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var testRoleName = new AppRoleName("Test");
            await app.AddRole(testRoleName);
            var roles = (await app.Roles()).ToArray();
            Assert.That(roles.Select(r => r.Name()), Has.One.EqualTo(testRoleName), "Should add role to app");
        }

        [Test]
        public async Task ShouldAddRoleToUser()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var adminRoleName = new AppRoleName("Admin");
            var adminRole = await app.AddRole(adminRoleName);
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<Clock>();
            var user = await factory.Users().Add
            (
                new AppUserName("someone"), new FakeHashedPassword("Password"), clock.Now()
            );
            await user.AddRole(adminRole);
            var defaultModifier = await app.DefaultModifier();
            var userRoles = await user.AssignedRoles(defaultModifier);
            Assert.That(userRoles.Length, Is.EqualTo(1), "Should add role to user");
            Assert.That(userRoles[0].ID.Equals(adminRole.ID), Is.True, "Should add role to user");
        }

        [Test]
        public async Task ShouldNotAddRoleFromAppRoleNames_WhenTheRoleAlreadyExists()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var roleNames = new[] { AppRoleName.Admin, FakeInfo.Roles.Manager, FakeInfo.Roles.Viewer };
            await app.SetRoles(roleNames);
            await app.SetRoles(roleNames);
            var appRoles = await app.Roles();
            Assert.That(appRoles.Select(r => r.Name()), Is.EquivalentTo(roleNames), "Should add role names from app role names");
        }

        [Test]
        public async Task ShouldRemoveRolesNotInAppRoleNames()
        {
            var services = await setup();
            var app = await services.FakeApp();
            var roleNames = new[] { AppRoleName.Admin, FakeInfo.Roles.Manager, FakeInfo.Roles.Viewer };
            await app.SetRoles(roleNames);
            roleNames = roleNames.Where(rn => !rn.Equals(FakeAppRoles.Instance.Manager)).ToArray();
            await app.SetRoles(roleNames);
            var appRoles = await app.Roles();
            Assert.That(appRoles.Select(r => r.Name()), Is.EquivalentTo(roleNames), "Should add role names from app role names");
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
            var factory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            await sp.Setup();
            return sp;
        }
    }
}
