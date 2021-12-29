﻿using XTI_App.Abstractions;

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
            AddModifier(ModifierKey.Default, "");
        }
    }

    public EntityID ID { get; }

    public ModifierCategoryName Name() => categoryName;

    public FakeModifier AddModifier(ModifierKey modKey, string targetID)
    {
        var mod = modifiers.FirstOrDefault(m => m.ModKey().Equals(modKey));
        if (mod == null)
        {
            mod = new FakeModifier(app, FakeModifier.NextID(), modKey, targetID);
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