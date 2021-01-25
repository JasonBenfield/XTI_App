﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Fakes;
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
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<Clock>();
            await factory.Users().Add
            (
                userName,
                new FakeHashedPassword("Password12345"),
                new PersonName("Test User"),
                new EmailAddress("test.user@hotmail.com"),
                clock.Now()
            );
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
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<Clock>();
            await factory.Users().Add
            (
                userName,
                new FakeHashedPassword("Password12345"),
                new PersonName("Test User"),
                new EmailAddress("test.user@hotmail.com"),
                clock.Now()
            );
            var users = (await factory.Users().Users()).ToArray();
            Assert.That(users.Select(u => u.UserName()), Has.One.EqualTo(userName), "Should get all users");
        }

        [Test]
        public async Task ShouldEditUser()
        {
            var services = await setup();
            var userName = new AppUserName("Test.User");
            var factory = services.GetService<AppFactory>();
            var clock = services.GetService<Clock>();
            await factory.Users().Add
            (
                userName,
                new FakeHashedPassword("Password12345"),
                new PersonName("Test User"),
                new EmailAddress("test.user@hotmail.com"),
                clock.Now()
            );
            var user = await factory.Users().User(userName);
            await user.Edit(new PersonName("Changed Name"), new EmailAddress("changed@gmail.com"));
            user = await factory.Users().User(userName);
            var userModel = user.ToModel();
            Assert.That(userModel.Name, Is.EqualTo("Changed Name"), "Should update user");
            Assert.That(userModel.Email, Is.EqualTo("changed@gmail.com"), "Should update user");
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
