using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class UnknownAppSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;

        public UnknownAppSetup(AppFactory appFactory, Clock clock)
        {
            this.appFactory = appFactory;
            this.clock = clock;
        }

        public async Task Run()
        {
            var anonUser = await appFactory.Users().User(AppUserName.Anon);
            if (anonUser.ID.IsNotValid())
            {
                await appFactory.Users().Add
                (
                    AppUserName.Anon,
                    new AnonHashedPassword(),
                    clock.Now()
                );
            }
            var setup = new SingleAppSetup
            (
                appFactory,
                clock,
                AppKey.Unknown,
                AppType.Values.NotFound,
                "",
                new AppRoleName[] { }
            );
            await setup.Run();
            var app = await appFactory.Apps().App(AppKey.Unknown, AppType.Values.NotFound);
            var group = await app.Group(ResourceGroupName.Unknown);
            if (group.ID.IsNotValid())
            {
                group = await app.AddGroup(ResourceGroupName.Unknown);
            }
            var resource = await group.Resource(ResourceName.Unknown);
            if (resource.ID.IsNotValid())
            {
                await group.AddResource(ResourceName.Unknown);
            }
        }

        private class AnonHashedPassword : IHashedPassword
        {
            public bool Equals(string other) => false;

            public string Value() => "ANON";
        }
    }
}
