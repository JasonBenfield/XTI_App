using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeModifierCategory : IModifierCategory
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static int NextID() => currentID.Next();

    private readonly FakeApp app;
    private readonly ModifierCategoryName categoryName;
    private readonly List<FakeModifier> modifiers = new List<FakeModifier>();

    public FakeModifierCategory(FakeApp app, int id, ModifierCategoryName categoryName)
    {
        this.app = app;
        ID = id;
        this.categoryName = categoryName;
        if (categoryName.Equals(ModifierCategoryName.Default))
        {
            AddModifier(getUniqueID(), ModifierKey.Default, "");
        }
    }

    private int getUniqueID()
    {
        var id = FakeAppUser.NextID();
        while (modifiers.Any(u => u.ID.Equals(id)))
        {
            id = FakeAppUser.NextID();
        }
        return id;
    }

    public int ID { get; }

    public ModifierCategoryName Name() => categoryName;

    public FakeModifier AddModifier(ModifierKey modKey, string targetID) =>
        AddModifier(getUniqueID(), modKey, targetID);

    public FakeModifier AddModifier(int id, ModifierKey modKey, string targetID)
    {
        var mod = modifiers.FirstOrDefault(m => m.ModKey().Equals(modKey));
        if (mod == null)
        {
            mod = new FakeModifier(id, modKey, targetID);
            modifiers.Add(mod);
        }
        return mod;
    }

    Task<IModifier> IModifierCategory.ModifierOrDefault(ModifierKey modKey) =>
        Task.FromResult<IModifier>(ModifierOrDefault(modKey));

    public FakeModifier ModifierOrDefault(ModifierKey modKey)
    {
        var mod = modifiers.FirstOrDefault(m => m.ModKey().Equals(modKey));
        if (mod == null)
        {
            var modCategory = app.ModCategory(ModifierCategoryName.Default);
            mod = modCategory.ModifierOrDefault(ModifierKey.Default);
        }
        return mod ?? throw new ArgumentNullException("mod");
    }

    public FakeModifier ModifierByTargetID(string targetID)
    {
        var mod = modifiers.FirstOrDefault(m => m.TargetID.Equals(targetID, StringComparison.OrdinalIgnoreCase));
        return mod ?? throw new ArgumentNullException("mod");
    }

    public FakeModifier[] Modifiers() => modifiers.ToArray();
}