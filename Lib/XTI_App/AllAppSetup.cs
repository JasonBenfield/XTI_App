using System.Threading.Tasks;
using XTI_Core;

namespace XTI_App
{
    public sealed class AllAppSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly Clock clock;

        public AllAppSetup(AppFactory appFactory, Clock clock)
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
                "",
                new AppRoleName[] { }
            );
            await setup.Run();
            var app = await appFactory.Apps().App(AppKey.Unknown);
            var defaultModCategory = await app.ModCategory(ModifierCategoryName.Default);
            await defaultModCategory.TryAddDefaultModifier();
            var group = await app.AddOrUpdateResourceGroup(ResourceGroupName.Unknown, defaultModCategory);
            await group.TryAddResource(ResourceName.Unknown);
        }

        private class AnonHashedPassword : IHashedPassword
        {
            public bool Equals(string other) => false;

            public string Value() => "ANON";
        }
    }
}
