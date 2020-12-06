using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;
        private readonly FakeAppOptions options;

        public FakeAppSetup(AppFactory appFactory, Clock clock, FakeAppOptions options = null)
        {
            this.appFactory = appFactory;
            this.clock = clock;
            this.options = options ?? new FakeAppOptions();
        }

        public App App { get; private set; }
        public AppVersion CurrentVersion { get; private set; }
        public AppUser User { get; private set; }

        public async Task Run()
        {
            var fakeTemplateFactory = new FakeAppApiTemplateFactory();
            var template = fakeTemplateFactory.Create();
            var setup = new DefaultAppSetup
            (
                appFactory,
                clock,
                template,
                options.Title,
                options.RoleNames.Values(),
                new[] { new ModifierCategoryName("Department") }
            );
            await setup.Run();
            App = await appFactory.Apps().App(template.AppKey);
            var modCategory = await App.ModCategory(new ModifierCategoryName("Department"));
            await modCategory.AddOrUpdateModifier(1, "IT");
            await modCategory.AddOrUpdateModifier(2, "HR");
            User = await appFactory.Users().Add
            (
                new AppUserName("xartogg"), new FakeHashedPassword("password"), clock.Now()
            );
        }
    }
}
