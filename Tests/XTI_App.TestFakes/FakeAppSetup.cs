using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.EfApi;
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

        public async Task Run(AppVersionKey versionKey)
        {
            var fakeApiFactory = new FakeAppApiFactory();
            var template = fakeApiFactory.CreateTemplate();
            var setup = new DefaultAppSetup
            (
                appFactory,
                clock,
                template,
                options.Title
            );
            await setup.Run(versionKey);
            App = await appFactory.Apps().App(template.AppKey);
            var modCategory = await App.TryAddModCategory(new ModifierCategoryName("Department"));
            await modCategory.AddOrUpdateModifier("IT", "IT");
            await modCategory.AddOrUpdateModifier("HR", "HR");
            var userName = new AppUserName("xartogg");
            User = await appFactory.Users().User(userName);
            if (!User.UserName().Equals(userName))
            {
                User = await appFactory.Users().Add
                (
                    userName, new FakeHashedPassword("password"), clock.Now()
                );
            }
        }
    }
}
