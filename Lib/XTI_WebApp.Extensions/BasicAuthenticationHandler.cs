using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using XTI_App.Abstractions;
using XTI_TempLog;
using XTI_TempLog.Abstractions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IMemoryCache cache;
    private readonly TempLogSession tempLog;
    private readonly IBasicAuthValidator basicAuth;

    public BasicAuthenticationHandler
    (
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        TempLogSession tempLog,
        IBasicAuthValidator basicAuth
    )
    : base(options, logger, encoder)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.cache = cache;
        this.tempLog = tempLog;
        this.basicAuth = basicAuth;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authResult = AuthenticateResult.NoResult();
        string? auth = httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
        if (!string.IsNullOrWhiteSpace(auth))
        {
            var cacheKey = $"xti_basicAuth_{auth}";
            if (!cache.TryGetValue<CachedClaims>(cacheKey, out var cachedClaims))
            {
                cachedClaims = new CachedClaims(new(), "");
                var creds = BasicAuthenticationCredentials.Parse(auth);
                if (!string.IsNullOrWhiteSpace(creds.UserName))
                {
                    var isValid = await basicAuth.IsValid(creds.UserName, creds.Password);
                    if (isValid)
                    {
                        var authSession = await tempLog.AuthenticateSession(creds.UserName);
                        cachedClaims = new CachedClaims(authSession.SessionKey, creds.UserName);
                        cache.Set
                        (
                            cacheKey,
                            cachedClaims,
                            new MemoryCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                                SlidingExpiration = TimeSpan.FromHours(1)
                            }
                        );
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(cachedClaims?.UserName))
            {
                authResult = AuthenticateResult.Fail("Basic Auth credentials were not valid");
            }
            else
            {
                var claims = new XtiClaimsCreator(cachedClaims.SessionKey, new AppUserName(cachedClaims.UserName)).Values();
                var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.NameIdentifier, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                authResult = AuthenticateResult.Success(ticket);
            }
        }
        return authResult;
    }

    private sealed record CachedClaims(SessionKey SessionKey, string UserName);
}
