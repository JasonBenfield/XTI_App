using XTI_App.Abstractions;

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

    public Task AddRole(FakeAppRole roleToAdd)
        => AddRoles(new[] { roleToAdd });

    public async Task AddRoles(params FakeAppRole[] rolesToAdd)
    {
        var app = await appContext.App();
        var defaultModCategory = await app.ModCategory(ModifierCategoryName.Default);
        var defaultModifier = await defaultModCategory.Modifier(ModifierKey.Default);
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

    public async Task<IAppRole[]> Roles(IModifier modifier)
    {
        if (!roles.TryGetValue(modifier.ID.Value, out var userRoles))
        {
            if (modifier.ModKey().Equals(ModifierKey.Default))
            {
                userRoles = new FakeAppRole[] { };
            }
            else
            {
                var app = await appContext.App();
                var defaultModCategory = await app.ModCategory(ModifierCategoryName.Default);
                var defaultMod = await defaultModCategory.Modifier(ModifierKey.Default);
                userRoles = (FakeAppRole[])await Roles(defaultMod);
            }
        }
        return userRoles;
    }

}