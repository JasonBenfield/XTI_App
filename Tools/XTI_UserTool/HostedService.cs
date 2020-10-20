﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XTI_App;
using XTI_Core;
using XTI_Credentials;
using XTI_Secrets;

namespace XTI_UserApp
{
    public sealed class HostedService : IHostedService
    {
        private readonly IHostApplicationLifetime lifetime;
        private readonly AppFactory appFactory;
        private readonly IHashedPasswordFactory hashedPasswordFactory;
        private readonly SecretCredentialsFactory secretCredentialsFactory;
        private readonly Clock clock;
        private readonly UserOptions userOptions;

        public HostedService
        (
            IHostApplicationLifetime lifetime,
            AppFactory appFactory,
            IHashedPasswordFactory hashedPasswordFactory,
            SecretCredentialsFactory secretCredentialsFactory,
            Clock clock,
            IOptions<UserOptions> userOptions
        )
        {
            this.lifetime = lifetime;
            this.appFactory = appFactory;
            this.hashedPasswordFactory = hashedPasswordFactory;
            this.secretCredentialsFactory = secretCredentialsFactory;
            this.clock = clock;
            this.userOptions = userOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var serialized = JsonSerializer.Serialize(userOptions);
            Console.WriteLine($"Args: {serialized}");
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            try
            {
                string password;
                if (string.IsNullOrWhiteSpace(userOptions.Password))
                {
                    password = Guid.NewGuid().ToString("N") + "!?";
                }
                else
                {
                    password = userOptions.Password;
                }
                var userName = new AppUserName(userOptions.UserName);
                var hashedPassword = hashedPasswordFactory.Create(password);
                var user = await appFactory.Users().User(userName);
                if (user.Exists())
                {
                    await user.ChangePassword(hashedPassword);
                }
                else
                {
                    user = await appFactory.Users().Add(userName, hashedPassword, clock.Now());
                }
                var roles = new List<AppRole>();
                if (!string.IsNullOrWhiteSpace(userOptions.AppKey))
                {
                    var appType = string.IsNullOrWhiteSpace(userOptions.AppType)
                        ? AppType.Values.WebApp
                        : AppType.Values.Value(userOptions.AppType);
                    var app = await appFactory.Apps().App(new AppKey(userOptions.AppKey), appType);
                    if (!string.IsNullOrWhiteSpace(userOptions.RoleNames))
                    {
                        foreach (var roleName in userOptions.RoleNames.Split(","))
                        {
                            if (!string.IsNullOrWhiteSpace(roleName))
                            {
                                var role = await app.Role(new AppRoleName(roleName));
                                if (role.Exists())
                                {
                                    roles.Add(role);
                                }
                            }
                        }
                    }
                    var userRoles = (await user.RolesForApp(app)).ToArray();
                    foreach (var role in roles)
                    {
                        if (!userRoles.Any(ur => ur.IsRole(role)))
                        {
                            await user.AddRole(role);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(userOptions.CredentialKey))
                {
                    var secretCredentials = secretCredentialsFactory.Create(userOptions.CredentialKey);
                    await secretCredentials.Update
                    (
                        new CredentialValue(userOptions.UserName, password)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.ExitCode = 999;
            }
            lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}