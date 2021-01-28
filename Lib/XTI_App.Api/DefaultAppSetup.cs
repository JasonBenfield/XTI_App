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

        public async Task Run(AppVersionKey versionKey)
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
            var version = await app.Version(versionKey);
            foreach (var groupTemplate in appTemplate.GroupTemplates)
            {
                await updateResourceGroupFromTemplate(app, version, groupTemplate);
            }
        }

        private static async Task updateResourceGroupFromTemplate(App app, AppVersion version, AppApiGroupTemplate groupTemplate)
        {
            var modCategory = await app.TryAddModCategory(groupTemplate.ModCategory);
            var groupName = new ResourceGroupName(groupTemplate.Name);
            var resourceGroup = await version.AddOrUpdateResourceGroup(groupName, modCategory);
            if (groupTemplate.Access.IsAnonymousAllowed)
            {
                await resourceGroup.AllowAnonymous();
            }
            else
            {
                await resourceGroup.DenyAnonymous();
            }
            var allowedGroupRoles = await rolesFromNames(app, groupTemplate.Access.Allowed);
            var deniedGroupRoles = await rolesFromNames(app, groupTemplate.Access.Denied);
            await resourceGroup.SetRoleAccess(allowedGroupRoles, deniedGroupRoles);
            foreach (var actionTemplate in groupTemplate.ActionTemplates)
            {
                await updateResourceFromTemplate(app, resourceGroup, actionTemplate);
            }
        }

        private static async Task updateResourceFromTemplate(App app, ResourceGroup resourceGroup, AppApiActionTemplate actionTemplate)
        {
            var resourceName = new ResourceName(actionTemplate.Name);
            var resource = await resourceGroup.TryAddResource(resourceName, actionTemplate.ResultType());
            if (actionTemplate.Access.IsAnonymousAllowed)
            {
                await resource.AllowAnonymous();
            }
            else
            {
                await resource.DenyAnonymous();
            }
            var allowedResourceRoles = await rolesFromNames(app, actionTemplate.Access.Allowed);
            var deniedResourceRoles = await rolesFromNames(app, actionTemplate.Access.Denied);
            await resource.SetRoleAccess(allowedResourceRoles, deniedResourceRoles);
        }

        private static async Task<IEnumerable<AppRole>> rolesFromNames(App app, IEnumerable<AppRoleName> roleNames)
        {
            var roles = new List<AppRole>();
            foreach (var roleName in roleNames)
            {
                var role = await app.Role(roleName);
                roles.Add(role);
            }
            return roles;
        }
    }
}
