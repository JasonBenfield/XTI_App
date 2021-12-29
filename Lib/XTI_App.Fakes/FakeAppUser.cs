﻿using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeAppUser : IAppUser
{
    private static FakeEntityID currentID = new FakeEntityID();
    public static EntityID NextID() => currentID.Next();

    private readonly FakeAppContext appContext;
    private readonly AppUserName userName;
    private readonly Dictionary<int, FakeAppRole[]> roles = new Dictionary<int, FakeAppRole[]>();

    public FakeAppUser(FakeAppContext appContext, EntityID id, AppUserName userName)
    {
        this.appContext = appContext;
        ID = id;
        this.userName = userName;
    }

    public EntityID ID { get; }

    public AppUserName UserName() => userName;

    public void AddRole(AppRoleName roleToAdd)
        => AddRoles(new[] { roleToAdd });

    public void AddRoles(params AppRoleName[] roleNamesToAdd)
    {
        var app = appContext.App();
        var roles = new List<FakeAppRole>();
        foreach (var roleName in roleNamesToAdd)
        {
            var role = app.Role(roleName);
            roles.Add(role);
        }
        AddRoles(roles.ToArray());
    }

    public void AddRoles(IModifier modifier, params AppRoleName[] roleNamesToAdd)
    {
        var app = appContext.App();
        var roles = new List<FakeAppRole>();
        foreach (var roleName in roleNamesToAdd)
        {
            var role = app.Role(roleName);
            roles.Add(role);
        }
        AddRoles(modifier, roles.ToArray());
    }

    public void AddRole(FakeAppRole roleToAdd) => AddRoles(new[] { roleToAdd });

    public void AddRoles(params FakeAppRole[] rolesToAdd)
    {
        var app = appContext.App();
        var defaultModCategory = app.ModCategory(ModifierCategoryName.Default);
        var defaultModifier = defaultModCategory.ModifierOrDefault(ModifierKey.Default);
        AddRoles(defaultModifier, rolesToAdd);
    }

    public void AddRoles(IModifier modifier, params FakeAppRole[] rolesToAdd)
    {
        if (roles.ContainsKey(modifier.ID.Value))
        {
            var originalRoles = roles[modifier.ID.Value];
            roles[modifier.ID.Value] = originalRoles.Union(rolesToAdd).Distinct().ToArray();
        }
        else
        {
            roles.Add(modifier.ID.Value, rolesToAdd);
        }
    }

    Task<IAppRole[]> IAppUser.Roles(IModifier modifier) =>
        Task.FromResult<IAppRole[]>(Roles(modifier));

    public FakeAppRole[] Roles(IModifier modifier)
    {
        if (!roles.TryGetValue(modifier.ID.Value, out var userRoles))
        {
            if (modifier.ModKey().Equals(ModifierKey.Default))
            {
                userRoles = new FakeAppRole[] { };
            }
            else
            {
                var app = appContext.App();
                var defaultModCategory = app.ModCategory(ModifierCategoryName.Default);
                var defaultMod = defaultModCategory.ModifierOrDefault(ModifierKey.Default);
                userRoles = Roles(defaultMod);
            }
        }
        return userRoles;
    }

}