using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeModifierCategory : IModifierCategory
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static EntityID NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly ModifierCategoryName categoryName;
    private readonly List<FakeModifier> modifiers = new List<FakeModifier>();

    public FakeModifierCategory(FakeApp app, EntityID id, ModifierCategoryName categoryName)
    {
        this.app = app;
        ID = id;
        this.categoryName = categoryName;
        if (categoryName.Equals(ModifierCategoryName.Default))
        {
            AddModifier(ModifierKey.Default);
        }
    }

    public EntityID ID { get; }

    public ModifierCategoryName Name() => categoryName;

    public FakeModifier AddModifier(ModifierKey modKey)
    {
        var mod = modifiers.FirstOrDefault(m => m.ModKey().Equals(modKey));
        if (mod == null)
        {
            mod = new FakeModifier(app, FakeModifier.NextID(), modKey);
            modifiers.Add(mod);
        }
        return mod;
    }

    async Task<IModifier> IModifierCategory.Modifier(ModifierKey modKey) => await Modifier(modKey);

    public async Task<FakeModifier> Modifier(ModifierKey modKey)
    {
        var mod = modifiers.FirstOrDefault(m => m.ModKey().Equals(modKey));
        if (mod == null)
        {
            var modCategory = await app.ModCategory(ModifierCategoryName.Default);
            mod = await modCategory.Modifier(ModifierKey.Default);
        }
        return mod;
    }
}