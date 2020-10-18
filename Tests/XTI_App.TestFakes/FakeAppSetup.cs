using System.Threading.Tasks;
using XTI_App.Fakes;
using XTI_Core;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;

        public FakeAppSetup(AppFactory appFactory, Clock clock)
        {
            this.appFactory = appFactory;
            this.clock = clock;
        }

        public App App { get; private set; }
        public AppVersion CurrentVersion { get; private set; }
        public AppUser User { get; private set; }

        public async Task Run()
        {
            App = await appFactory.Apps().AddApp(new AppKey("Fake"), AppType.Values.WebApp, "Fake", clock.Now());
            var version = await App.StartNewPatch(clock.Now());
            await version.Publishing();
            await version.Published();
            CurrentVersion = await App.CurrentVersion();
            User = await appFactory.Users().Add
            (
                new AppUserName("xartogg"), new FakeHashedPassword("password"), clock.Now()
            );
            foreach (var roleName in FakeAppRoles.Instance.Values())
            {
                await App.AddRole(roleName);
            }
        }
    }
}
