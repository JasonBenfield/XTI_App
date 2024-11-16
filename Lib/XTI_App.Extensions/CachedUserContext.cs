using Microsoft.Extensions.Caching.Memory;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class CachedUserContext : ICachedUserContext
{
    private readonly IMemoryCache cache;
    private readonly ISourceUserContext sourceUserContext;
    private readonly ICurrentUserName currentUserName;

    public CachedUserContext(IMemoryCache cache, ISourceUserContext sourceUserContext, ICurrentUserName currentUserName)
    {
        this.cache = cache;
        this.sourceUserContext = sourceUserContext;
        this.currentUserName = currentUserName;
    }

    public void ClearCache(AppUserName userName)
    {
        cache.Remove(GetUserCacheKey(userName));
        cache.Remove(GetUserRolesCacheKey(userName));
    }

    private static string GetUserCacheKey(AppUserName userName) => $"xti_user_{userName.Value}";

    public async Task<AppUserModel> User()
    {
        var userName = await currentUserName.Value();
        var cachedUser = await User(userName);
        return cachedUser;
    }

    public async Task<AppUserModel> User(AppUserName userName)
    {
        var cacheKey = GetUserCacheKey(userName);
        if (!cache.TryGetValue<AppUserModel>(cacheKey, out var cachedUser))
        {
            cachedUser = await sourceUserContext.User(userName);
            if (!userName.IsAnon() && cachedUser.IsAnon())
            {
                throw new Exception($"User '{userName.DisplayText}' was not found.");
            }
            cache.Set
            (
                cacheKey,
                cachedUser,
                TimeSpan.FromHours(1)
            );
        }
        return cachedUser ?? new AppUserModel();
    }

    public async Task<AppUserModel> UserOrAnon(AppUserName userName)
    {
        var cacheKey = GetUserCacheKey(userName);
        if (!cache.TryGetValue<AppUserModel>(cacheKey, out var cachedUser))
        {
            cachedUser = await sourceUserContext.UserOrAnon(userName);
            cache.Set
            (
                cacheKey,
                cachedUser,
                TimeSpan.FromHours(1)
            );
        }
        return cachedUser ?? new AppUserModel();
    }

    public async Task<AppRoleModel[]> UserRoles(AppUserModel user, ModifierModel modifier)
    {
        var cacheKey = GetUserRolesCacheKey(user.UserName);
        if (!cache.TryGetValue<List<ModifiedUserRoles>>(cacheKey, out var modifiedUserRoles))
        {
            modifiedUserRoles = new List<ModifiedUserRoles>();
            cache.Set
            (
                cacheKey,
                modifiedUserRoles,
                TimeSpan.FromHours(1)
            );
        }
        if (modifiedUserRoles == null)
        {
            modifiedUserRoles = new();
        }
        var modifiedUserRole = modifiedUserRoles.FirstOrDefault(mur => mur.Modifier.ID == modifier.ID);
        if (modifiedUserRole == null)
        {
            var userRoles = await sourceUserContext.UserRoles(user, modifier);
            modifiedUserRole = new ModifiedUserRoles(modifier, userRoles);
            modifiedUserRoles.Add(modifiedUserRole);
        }
        return modifiedUserRole.UserRoles;
    }

    private static string GetUserRolesCacheKey(AppUserName userName) =>
        $"xti_user_{userName.Value}_roles";

    private sealed record ModifiedUserRoles(ModifierModel Modifier, AppRoleModel[] UserRoles);
}