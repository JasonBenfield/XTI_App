using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed class UserMenuDefinition
{
    public UserMenuDefinition()
    {
        HomeLink = new LinkModel("home", "Home", "~/");
        UserLink = new LinkModel("user", "{User.FullName}", "~/User/UserProfile");
        LogoutLink = new LinkModel("logout", "Logout", "~/User/Logout");
        Value = new MenuDefinition
        (
            "user",
            HomeLink,
            UserLink,
            LogoutLink
        );
    }

    public MenuDefinition Value { get; }
    public LinkModel HomeLink { get; }
    public LinkModel UserLink { get; }
    public LinkModel LogoutLink { get; }

    public MenuDefinition Modify(params LinkModel[] links) =>
        new MenuDefinition(Value.MenuName, links);
}
