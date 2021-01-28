using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class SingleAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly AppKey appKey;
        private readonly string appTitle;
        private readonly IEnumerable<AppRoleName> roleNames;

        public SingleAppSetup(AppFactory appFactory, Clock clock, AppKey appKey, string appTitle, IEnumerable<AppRoleName> roleNames)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.appKey = appKey;
            this.appTitle = appTitle;
            this.roleNames = roleNames;
        }

        public async Task Run()
        {
            var app = await appFactory.Apps().AddOrUpdate(appKey, appTitle, clock.Now());
            var currentVersion = await app.CurrentVersion();
            if (!currentVersion.IsCurrent())
            {
                currentVersion = await app.StartNewMajorVersion(clock.Now());
                await currentVersion.Publishing();
                await currentVersion.Published();
            }
            await app.SetRoles(roleNames);
            var defaultModCategory = await app.TryAddModCategory(ModifierCategoryName.Default);
            var group = await currentVersion.AddOrUpdateResourceGroup(ResourceGroupName.Unknown, defaultModCategory);
            await group.TryAddResource(ResourceName.Unknown, ResourceResultType.Values.None);
        }

    }
}
