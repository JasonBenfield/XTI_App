namespace XTI_App.Abstractions;

public sealed record AppUserModel
(
    int ID,
    AppUserName UserName,
    PersonName Name,
    string Email,
    DateTimeOffset TimeDeactivated
)
{
    public AppUserModel()
        : this(0, AppUserName.Anon, new PersonName(""), "", DateTimeOffset.MaxValue)
    {
    }

    public bool IsAnon() => UserName.IsAnon();

    public bool IsActive() => TimeDeactivated.Year == DateTimeOffset.MaxValue.Year;
}