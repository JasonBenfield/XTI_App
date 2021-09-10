using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeModifier : IModifier
    {
        private static FakeEntityID currentID = new FakeEntityID();
        public static EntityID NextID() => currentID.Next();

        private readonly FakeApp app;
        private readonly ModifierKey modKey;

        public FakeModifier(FakeApp app, EntityID id, ModifierKey modKey)
        {
            this.app = app;
            ID = id;
            this.modKey = modKey;
        }

        public EntityID ID { get; }

        public ModifierKey ModKey() => modKey;

        public async Task<IModifier> DefaultModifier()
        {
            var category = await app.ModCategory(ModifierCategoryName.Default);
            var modifier = await category.Modifier(ModifierKey.Default);
            return modifier;
        }

    }
}
