namespace XTI_WebApp.Api;

public sealed class UserMenuDefinition
{
    public UserMenuDefinition()
    {
        HomeLink = new("home", "Home", "~/", true);
        UserLink = new("user", "{User.FullName}", "~/User/UserProfile");
        LogoutLink = new("logout", "Logout", "~/User/Logout");
        Value = new
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
        new(Value.MenuName, links);
}
