using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class DefaultAppSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly AppKey appKey;
        private readonly AppType appType;
        private readonly string appTitle;
        private readonly IEnumerable<AppRoleName> roleNames;

        public DefaultAppSetup(AppFactory appFactory, Clock clock, AppKey appKey, AppType appType, string appTitle, IEnumerable<AppRoleName> roleNames)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.appKey = appKey;
            this.appType = appType;
            this.appTitle = appTitle;
            this.roleNames = roleNames;
        }

        public async Task Run()
        {
            var userRepo = appFactory.Users();
            var anonUser = await userRepo.User(AppUserName.Anon);
            if (!anonUser.Exists())
            {
                await userRepo.Add
                (
                    AppUserName.Anon,
                    new AnonHashedPassword(),
                    new UtcClock().Now()
                );
            }
            var app = await appFactory.Apps().App(appKey, appType);
            if (app.Exists())
            {
                await app.SetTitle(appTitle);
            }
            else
            {
                app = await appFactory.Apps().AddApp(appKey, appType, appTitle, clock.Now());
            }
            var currentVersion = await app.CurrentVersion();
            if (!currentVersion.IsCurrent())
            {
                currentVersion = await app.StartNewMajorVersion(clock.Now());
                await currentVersion.Publishing();
                await currentVersion.Published();
            }
            await app.SetRoles(roleNames);
        }

        private class AnonHashedPassword : IHashedPassword
        {
            public bool Equals(string other) => false;

            public string Value() => "ANON";
        }
    }
}
