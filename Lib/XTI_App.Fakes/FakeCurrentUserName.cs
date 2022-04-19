using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeCurrentUserName : ICurrentUserName
{
    private AppUserName userName;

    public FakeCurrentUserName()
        : this(AppUserName.Anon)
    {
    }

    public FakeCurrentUserName(AppUserName userName)
    {
        this.userName = userName;
    }

    public Task<AppUserName> Value() => Task.FromResult(GetUserName());

    public AppUserName GetUserName() => userName;

    public void SetUserName(AppUserName userName) => this.userName = userName;
}
