using System.Collections.Generic;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App.Api
{
    public sealed class DefaultAppSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly AppApiTemplate appTemplate;
        private readonly string appTitle;
        private readonly IEnumerable<AppRoleName> roleNames;
        private readonly IEnumerable<ModifierCategoryName> modCategoryNames;

        public DefaultAppSetup
        (
            AppFactory appFactory,
            Clock clock,
            AppApiTemplate appTemplate,
            string appTitle,
            IEnumerable<AppRoleName> roleNames,
            IEnumerable<ModifierCategoryName> modCategoryNames
        )
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.appTemplate = appTemplate;
            this.appTitle = appTitle;
            this.roleNames = roleNames;
            this.modCategoryNames = modCategoryNames;
        }

        public async Task Run()
        {
            var allAppSetup = new AllAppSetup(appFactory, clock);
            await allAppSetup.Run();
            var appSetup = new SingleAppSetup
            (
                appFactory,
                clock,
                appTemplate.AppKey,
                appTitle,
                roleNames
            );
            await appSetup.Run();
            var app = await appFactory.Apps().App(appTemplate.AppKey);
            foreach (var modCategoryName in modCategoryNames)
            {
                await app.TryAddModCategory(modCategoryName);
            }
            foreach (var groupTemplate in appTemplate.GroupTemplates)
            {
                var modCategory = await app.ModCategory(groupTemplate.ModCategory);
                var groupName = new ResourceGroupName(groupTemplate.Name);
                var resourceGroup = await app.AddOrUpdateResourceGroup(groupName, modCategory);
                foreach (var actionTemplate in groupTemplate.ActionTemplates)
                {
                    var resourceName = new ResourceName(actionTemplate.Name);
                    await resourceGroup.TryAddResource(resourceName);
                }
            }
        }

    }
}
