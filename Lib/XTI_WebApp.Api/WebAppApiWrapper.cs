using XTI_App.Api;

namespace XTI_WebApp.Api;

public class WebAppApiWrapper : AppApiWrapper
{
    protected WebAppApiWrapper(AppApi source)
        : base(source)
    {
        User = new UserGroup
        (
            source.AddGroup
            (
                nameof(User),
                ModifierCategoryName.Default,
                ResourceAccess.AllowAuthenticated()
            )
        );
        UserCache = new UserCacheGroup
        (
            source.AddGroup
            (
                nameof(UserCache),
                ModifierCategoryName.Default,
                ResourceAccess.AllowAuthenticated()
                    .WithAllowed(AppRoleName.ManageUserCache)
            )
        );
    }

    public UserGroup User { get; }
    public UserCacheGroup UserCache { get; }
}