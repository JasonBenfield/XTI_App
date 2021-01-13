using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class LoginGroup : AppApiGroup
    {
        public LoginGroup(AppApi api, IAppApiUser user)
            : base
            (
                  api,
                  new NameFromGroupClassName(nameof(LoginGroup)).Value,
                  ModifierCategoryName.Default,
                  ResourceAccess.AllowAnonymous(),
                  user,
                  (p, a, u) => new AppApiActionCollection(p, a, u)
            )
        {
            var actions = Actions<AppApiActionCollection>();
            Authenticate = actions.Add
            (
                nameof(Authenticate),
                () => new EmptyAppAction<EmptyRequest, EmptyActionResult>()
            );
        }
        public AppApiAction<EmptyRequest, EmptyActionResult> Authenticate { get; }
    }

}
