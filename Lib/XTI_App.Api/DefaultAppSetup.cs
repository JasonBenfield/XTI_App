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

        public DefaultAppSetup(AppFactory appFactory, Clock clock, AppApiTemplate appTemplate, string appTitle, IEnumerable<AppRoleName> roleNames)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.appTemplate = appTemplate;
            this.appTitle = appTitle;
            this.roleNames = roleNames;
        }

        public async Task Run()
        {
            var unknownAppSetup = new UnknownAppSetup(appFactory, clock);
            await unknownAppSetup.Run();
            var appKey = new AppKey(appTemplate.Name);
            var appSetup = new SingleAppSetup
            (
                appFactory,
                clock,
                appKey,
                appTemplate.AppType,
                appTitle,
                roleNames
            );
            await appSetup.Run();
            var app = await appFactory.Apps().App(appKey, appTemplate.AppType);
            foreach (var groupTemplate in appTemplate.GroupTemplates)
            {
                var groupName = new ResourceGroupName(groupTemplate.Name);
                var resourceGroup = await app.Group(groupName);
                if (!resourceGroup.Name().Equals(groupName))
                {
                    resourceGroup = await app.AddGroup(groupName);
                }
                foreach (var actionTemplate in groupTemplate.ActionTemplates)
                {
                    var resourceName = new ResourceName(actionTemplate.Name);
                    var resource = await resourceGroup.Resource(resourceName);
                    if (!resource.Name().Equals(resourceName))
                    {
                        await resourceGroup.AddResource(resourceName);
                    }
                }
            }
        }

    }
}
