using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeAppSetup : IAppSetup
    {
        private readonly AppApiFactory apiFactory;
        private readonly FakeAppContext appContext;
        private readonly FakeUserContext userContext;
        private readonly FakeAppOptions options;

        public FakeAppSetup(AppApiFactory apiFactory, FakeAppContext appContext, FakeUserContext userContext, FakeAppOptions options = null)
        {
            this.apiFactory = apiFactory;
            this.appContext = appContext;
            this.userContext = userContext;
            this.options = options ?? new FakeAppOptions();
        }

        public FakeApp App { get; private set; }
        public FakeAppVersion CurrentVersion { get; private set; }
        public FakeAppUser User { get; private set; }

        public async Task Run(AppVersionKey versionKey)
        {
            var template = apiFactory.CreateTemplate();
            var templateModel = template.ToModel();
            App = appContext.AddApp(options.Title);
            appContext.SetCurrentApp(App);
            var modCategories = templateModel.GroupTemplates.Select(g => g.ModCategory).Distinct();
            foreach (var modCategory in modCategories)
            {
                App.AddModCategory(new ModifierCategoryName(modCategory));
            }
            var roles = templateModel.RecursiveRoles();
            foreach (var role in roles)
            {
                var roleName = new AppRoleName(role);
                var existingRole = await App.Role(roleName);
                if (existingRole == null)
                {
                    App.AddRole(roleName);
                }
            }
            var version = await App.Version(versionKey);
            if (version == null)
            {
                version = App.AddVersion(versionKey);
            }
            foreach (var groupTemplate in templateModel.GroupTemplates)
            {
                var group = version.AddResourceGroup(new ResourceGroupName(groupTemplate.Name), new ModifierCategoryName(groupTemplate.ModCategory));
                foreach (var actionTemplate in groupTemplate.ActionTemplates)
                {
                    group.AddResource(new ResourceName(actionTemplate.Name));
                }
            }
            var departmentModCategory = await App.ModCategory(new ModifierCategoryName("Department"));
            departmentModCategory.AddModifier(new ModifierKey("IT"));
            departmentModCategory.AddModifier(new ModifierKey("HR"));
            var userName = new AppUserName("xartogg");
            User = userContext.AddUser(userName);
            userContext.SetCurrentUser(userName);
        }
    }
}
