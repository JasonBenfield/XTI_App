﻿using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeApp : IApp
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static EntityID NextID() => currentID.Next();

    private readonly List<FakeAppVersion> versions = new List<FakeAppVersion>();
    private readonly List<FakeAppRole> roles = new List<FakeAppRole>();
    private readonly List<FakeModifierCategory> modCategories = new List<FakeModifierCategory>();

    public FakeApp(EntityID id, string title)
    {
        ID = id;
        Title = title;
        AddVersion(AppVersionKey.Current);
        AddModCategory(ModifierCategoryName.Default);
        foreach (var roleName in AppRoleName.DefaultRoles())
        {
            AddRole(roleName);
        }
    }

    public EntityID ID { get; }
    public string Title { get; private set; }

    public void SetTitle(string title)
    {
        Title = title;
    }

    public FakeAppVersion CurrentVersion()
    {
        var version = versions.FirstOrDefault(v => v.Key().Equals(AppVersionKey.Current));
        if (version == null)
        {
            throw new ArgumentException("Current version not found");
        }
        return version;
    }

    public FakeAppVersion AddVersion(AppVersionKey versionKey)
    {
        var version = versions.FirstOrDefault(v => v.Key().Equals(versionKey));
        if (version == null)
        {
            version = new FakeAppVersion(this, FakeAppVersion.NextID(), versionKey);
            versions.Add(version);
        }
        return version;
    }

    async Task<IAppVersion> IApp.Version(AppVersionKey versionKey) => await Version(versionKey);

    public Task<FakeAppVersion> Version(AppVersionKey versionKey)
    {
        var version = versions.FirstOrDefault(v => v.Key().Equals(versionKey));
        if (version == null) { throw new ArgumentNullException("version"); }
        return Task.FromResult(version);
    }

    public FakeModifierCategory AddModCategory(ModifierCategoryName name)
    {
        var category = modCategories.FirstOrDefault(c => c.Name().Equals(name));
        if (category == null)
        {
            category = new FakeModifierCategory(this, FakeModifierCategory.NextID(), name);
            modCategories.Add(category);
        }
        return category;
    }

    Task<IModifierCategory> IApp.ModCategory(ModifierCategoryName name) =>
        Task.FromResult<IModifierCategory>(ModCategory(name));

    public FakeModifierCategory ModCategory(ModifierCategoryName name)
    {
        var category = modCategories.FirstOrDefault(c => c.Name().Equals(name));
        if (category == null)
        {
            throw new ArgumentException($"Category '{name.Value}' was not found");
        }
        return category;
    }

    public FakeAppRole AddRole(AppRoleName roleName)
    {
        var role = roles.FirstOrDefault(r => r.Name().Equals(roleName));
        if (role == null)
        {
            role = new FakeAppRole(FakeAppRole.NextID(), roleName);
            roles.Add(role);
        }
        return role;
    }

    public FakeAppRole Role(AppRoleName roleName) =>
        roles
            .FirstOrDefault(r => r.Name().Equals(roleName))
            ?? throw new ArgumentException($"Role '{roleName.Value}' was not found");

    Task<IAppRole[]> IApp.Roles() => Task.FromResult<IAppRole[]>(Roles());

    public FakeAppRole[] Roles() => roles.ToArray();

}