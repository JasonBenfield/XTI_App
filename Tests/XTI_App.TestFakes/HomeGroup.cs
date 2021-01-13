using XTI_App.Api;

namespace XTI_App.TestFakes
{
    public sealed class HomeGroup : AppApiGroup
    {
        public HomeGroup(AppApi api, IAppApiUser user)
            : base
            (
                  api,
                  new NameFromGroupClassName(nameof(HomeGroup)).Value,
                  ModifierCategoryName.Default,
                  ResourceAccess.AllowAuthenticated(),
                  user,
                  (n, a, u) => new AppApiActionCollection(n, a, u)
            )
        {
            var actions = Actions<AppApiActionCollection>();
            DoSomething = actions.Add
            (
                nameof(DoSomething),
                () => new EmptyAppAction<EmptyRequest, EmptyActionResult>()
            );
        }
        public AppApiAction<EmptyRequest, EmptyActionResult> DoSomething { get; }
    }

}
