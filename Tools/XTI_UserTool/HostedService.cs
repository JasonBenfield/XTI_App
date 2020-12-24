using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider services;

        public HostedService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            try
            {
                var userOptions = scope.ServiceProvider.GetService<IOptions<UserOptions>>().Value;
                if (userOptions.Command.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    await addUser(scope.ServiceProvider, userOptions);
                }
                else if (userOptions.Command.Equals("userroles", StringComparison.OrdinalIgnoreCase))
                {
                    await addUserRoles(scope.ServiceProvider, userOptions);
                }
                else if (userOptions.Command.Equals("credentials", StringComparison.OrdinalIgnoreCase))
                {
                    await storeCredentials(scope.ServiceProvider, userOptions);
                }
                else if (userOptions.Command.Equals("grant-modcategoryadmin", StringComparison.OrdinalIgnoreCase))
                {
                    var sp = scope.ServiceProvider;
                    await grantModCategoryAdmin(sp, userOptions);
                }
                else if (userOptions.Command.Equals("modifiers", StringComparison.OrdinalIgnoreCase))
                {
                    var sp = scope.ServiceProvider;
                    await addModifiers(sp, userOptions);
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
            var lifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            lifetime.StopApplication();
        }

        private static async Task addUser(IServiceProvider sp, UserOptions userOptions)
        {
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            var appFactory = sp.GetService<AppFactory>();
            var clock = sp.GetService<Clock>();
            var hashedPasswordFactory = sp.GetService<IHashedPasswordFactory>();
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
                await appFactory.Users().Add(userName, hashedPassword, clock.Now());
            }
        }

        private static async Task addUserRoles(IServiceProvider sp, UserOptions userOptions)
        {
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppName)) { throw new ArgumentException("App name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppType)) { throw new ArgumentException("App type is required"); }
            var appFactory = sp.GetService<AppFactory>();
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

        private static async Task storeCredentials(IServiceProvider sp, UserOptions userOptions)
        {
            if (string.IsNullOrWhiteSpace(userOptions.CredentialKey)) { throw new ArgumentException("Credential Key is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.Password)) { throw new ArgumentException("Password is required"); }
            var secretCredentialsFactory = sp.GetService<SecretCredentialsFactory>();
            var secretCredentials = secretCredentialsFactory.Create(userOptions.CredentialKey);
            await secretCredentials.Update
            (
                new CredentialValue(userOptions.UserName, userOptions.Password)
            );
        }

        private static async Task grantModCategoryAdmin(IServiceProvider sp, UserOptions userOptions)
        {
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppName)) { throw new ArgumentException("App name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppType)) { throw new ArgumentException("App type is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.ModCategoryName)) { throw new ArgumentException("Mod category name is required"); }
            var appFactory = sp.GetService<AppFactory>();
            var userName = new AppUserName(userOptions.UserName);
            var user = await appFactory.Users().User(userName);
            var appType = string.IsNullOrWhiteSpace(userOptions.AppType)
                ? AppType.Values.WebApp
                : AppType.Values.Value(userOptions.AppType);
            var app = await appFactory.Apps().App(new AppKey(userOptions.AppName, appType));
            var modCategory = await app.ModCategory(new ModifierCategoryName(userOptions.ModCategoryName));
            var isModCategoryAdmin = await user.IsModCategoryAdmin(modCategory);
            if (!isModCategoryAdmin)
            {
                await user.GrantFullAccessToModCategory(modCategory);
            }
        }

        private static async Task addModifiers(IServiceProvider sp, UserOptions userOptions)
        {
            if (string.IsNullOrWhiteSpace(userOptions.UserName)) { throw new ArgumentException("User name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppName)) { throw new ArgumentException("App name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.AppType)) { throw new ArgumentException("App type is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.ModCategoryName)) { throw new ArgumentException("Mod category name is required"); }
            if (string.IsNullOrWhiteSpace(userOptions.ModKeys)) { throw new ArgumentException("Mod keys is required"); }
            var appFactory = sp.GetService<AppFactory>();
            var userName = new AppUserName(userOptions.UserName);
            var user = await appFactory.Users().User(userName);
            var appType = string.IsNullOrWhiteSpace(userOptions.AppType)
                ? AppType.Values.WebApp
                : AppType.Values.Value(userOptions.AppType);
            var app = await appFactory.Apps().App(new AppKey(userOptions.AppName, appType));
            var modCategory = await app.ModCategory(new ModifierCategoryName("Apps"));
            var modKeys = userOptions.ModKeys
                .Split(',')
                .Select(m => new ModifierKey(m));
            var userModifiers = await user.Modifiers(modCategory);
            foreach (var modKey in modKeys)
            {
                if (!userModifiers.Any(um => um.ModKey().Equals(modKey)))
                {
                    var modifier = await modCategory.Modifier(modKey);
                    await user.AddModifier(modifier);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
