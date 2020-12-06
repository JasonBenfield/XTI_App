using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
            try
            {
                if (userOptions.Command.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
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
                }
                else if (userOptions.Command.Equals("userroles", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
                    if (string.IsNullOrWhiteSpace(userOptions.AppName)) { throw new ArgumentException("App name is required"); }
                    if (string.IsNullOrWhiteSpace(userOptions.AppType)) { throw new ArgumentException("App type is required"); }
                    var userName = new AppUserName(userOptions.UserName);
                    var user = await appFactory.Users().User(userName);
                    var appType = string.IsNullOrWhiteSpace(userOptions.AppType)
                        ? AppType.Values.WebApp
                        : AppType.Values.Value(userOptions.AppType);
                    var app = await appFactory.Apps().App(new AppKey(userOptions.AppName, appType));
                    var roles = new List<AppRole>();
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
                else if (userOptions.Command.Equals("credentials", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(userOptions.CredentialKey)) { throw new ArgumentException("Credential Key is required"); }
                    if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
                    if (string.IsNullOrWhiteSpace(userOptions.Password)) { throw new ArgumentException("Password is required"); }
                    var secretCredentials = secretCredentialsFactory.Create(userOptions.CredentialKey);
                    await secretCredentials.Update
                    (
                        new CredentialValue(userOptions.UserName, userOptions.Password)
                    );
                }
                else
                {
                    throw new NotSupportedException($"Command {userOptions.Command} is not supported");
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
