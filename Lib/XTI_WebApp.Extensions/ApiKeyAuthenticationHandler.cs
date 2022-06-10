using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using XTI_App.Abstractions;
using XTI_TempLog;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class ApiKeyHeaderOptions
{
    public string HeaderName { get; set; } = "";
    public ApiKeyOptions[] ApiKeys { get; set; } = new ApiKeyOptions[0];
}

public sealed class ApiKeyOptions
{
    public string UserName { get; set; } = "";
    public string ApiKey { get; set; } = "";
}

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiKeyAuthenticationOptions apiKeyAuth;
    private readonly TempLogSession tempLog;
    private readonly IMemoryCache cache;

    public ApiKeyAuthenticationHandler
    (
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IHttpContextAccessor httpContextAccessor,
        ApiKeyAuthenticationOptions apiKeyAuth,
        TempLogSession tempLog,
        IMemoryCache cache
    )
        : base(options, logger, encoder, clock)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.apiKeyAuth = apiKeyAuth;
        this.tempLog = tempLog;
        this.cache = cache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = AuthenticateResult.NoResult();
        var reqHeaderName = httpContextAccessor.HttpContext?.ApiKeyHeaderName();
        if (!string.IsNullOrWhiteSpace(reqHeaderName))
        {
            var reqHeader = httpContextAccessor.HttpContext?.Request.Headers[reqHeaderName];
            if (reqHeader.HasValue)
            {
                var authHeader = apiKeyAuth.Headers.First(h => h.HeaderName == reqHeaderName);
                var apiKey = authHeader.ApiKeys
                    .FirstOrDefault(key => key.ApiKey == reqHeader.Value);
                if (apiKey == null)
                {
                    result = AuthenticateResult.Fail($"API Key was not correct for header name {reqHeaderName}");
                }
                else
                {
                    var userName = new AppUserName(apiKey.UserName);
                    var cacheKey = $"xti_apiKeyAuth_{reqHeaderName}_{apiKey.ApiKey}";
                    if (!cache.TryGetValue<string>(cacheKey, out var sessionKey))
                    {
                        var authSession = await tempLog.AuthenticateSession(userName.Value);
                        sessionKey = authSession.SessionKey;
                        cache.Set
                        (
                            cacheKey,
                            sessionKey,
                            new MemoryCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                                SlidingExpiration = TimeSpan.FromHours(1)
                            }
                        );
                    }
                    var claims = new XtiClaimsCreator(sessionKey, userName).Values();
                    var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.NameIdentifier, ClaimTypes.Role);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    result = AuthenticateResult.Success(ticket);
                }
            }
        }
        return result;
    }
}
