using System.Threading.Tasks;
using XTI_App.Fakes;
using XTI_Core;

namespace XTI_App.TestFakes
{
    public sealed class FakeAppSetup : IAppSetup
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
            var setup = new DefaultAppSetup
            (
                appFactory,
                clock,
                new AppKey("Fake"),
                AppType.Values.WebApp,
                "Fake Title",
                FakeAppRoles.Instance.Values()
            );
            await setup.Run();
            App = await appFactory.Apps().App(new AppKey("Fake"), AppType.Values.WebApp);
            User = await appFactory.Users().Add
            (
                new AppUserName("xartogg"), new FakeHashedPassword("password"), clock.Now()
            );
        }
    }
}
