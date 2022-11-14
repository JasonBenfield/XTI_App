namespace XTI_App.Abstractions;

public sealed record AppUserModel
(
    int ID,
    AppUserName UserName,
    PersonName Name,
    string Email
)
{
    public AppUserModel()
        : this(0, AppUserName.Anon, new PersonName(""), "")
    {
    }

    public bool IsAnon() => UserName.IsAnon();
}