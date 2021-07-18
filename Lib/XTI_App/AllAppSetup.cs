using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AllAppSetup
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
                    new SystemHashedPassword(),
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
        }

        private class SystemHashedPassword : IHashedPassword
        {
            public bool Equals(string other) => false;

            public string Value() => new GeneratedKey().Value();
        }
    }
}
