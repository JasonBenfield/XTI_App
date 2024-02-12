using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestAuthOptions testOptions;

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, TestAuthOptions testOptions)
        : base(options, logger, encoder)
    {
        this.testOptions = testOptions;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult result;
        if (testOptions.IsEnabled)
        {
            var principal = new FakeHttpUser().Create
            (
                testOptions.SessionKey, 
                testOptions.User ?? throw new ArgumentNullException("testOptions.User")
            );
            var ticket = new AuthenticationTicket(principal, "Test");
            result = AuthenticateResult.Success(ticket);
        }
        else
        {
            result = AuthenticateResult.NoResult();
        }
        return Task.FromResult(result);
    }
}