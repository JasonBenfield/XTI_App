using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class UserProfileAction : AppAction<EmptyRequest, WebRedirectResult>
{
    private readonly IUserProfileUrl userProfileUrl;

    public UserProfileAction(IUserProfileUrl userProfileUrl)
    {
        this.userProfileUrl = userProfileUrl;
    }

    public async Task<WebRedirectResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        var url = await userProfileUrl.Value();
        return new WebRedirectResult(url);
    }
}
