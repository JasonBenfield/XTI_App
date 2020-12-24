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

        public DefaultAppSetup
        (
            AppFactory appFactory,
            Clock clock,
            AppApiTemplate appTemplate,
            string appTitle
        )
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.appTemplate = appTemplate;
            this.appTitle = appTitle;
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
                appTemplate.RoleNames
            );
            await appSetup.Run();
            var app = await appFactory.Apps().App(appTemplate.AppKey);
            foreach (var groupTemplate in appTemplate.GroupTemplates)
            {
                var modCategory = await app.TryAddModCategory(groupTemplate.ModCategory);
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
