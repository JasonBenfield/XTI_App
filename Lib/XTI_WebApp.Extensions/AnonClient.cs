using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using XTI_Core;
using XTI_Secrets;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;

namespace XTI_WebApp.Extensions;

public sealed class AnonClient : IAnonClient
{
    private readonly IDataProtector protector;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly AnonClientOptions anonOptions;

    public AnonClient(IDataProtector protector, IHttpContextAccessor httpContextAccessor, AnonClientOptions anonOptions)
    {
        this.protector = protector;
        this.httpContextAccessor = httpContextAccessor;
        this.anonOptions = anonOptions;
    }

    public string SessionKey { get; private set; } = "";
    public DateTimeOffset SessionExpirationTime { get; private set; } = DateTimeOffset.MinValue;
    public string RequesterKey { get; private set; } = "";

    public void Load()
    {
        var cookieText = httpContextAccessor.HttpContext?.Request.Cookies[anonOptions.CookieName];
        if (string.IsNullOrWhiteSpace(cookieText))
        {
            SessionKey = "";
            SessionExpirationTime = DateTimeOffset.MinValue;
            RequesterKey = "";
        }
        else
        {
            var unprotectedText = new DecryptedValue(protector, cookieText).Value();
            var info = XtiSerializer.Deserialize<AnonInfo>(unprotectedText);
            SessionKey = info.SessionKey;
            SessionExpirationTime = info.SessionExpirationTime;
            RequesterKey = info.RequesterKey;
        }
    }

    public void Persist(string sessionKey, DateTimeOffset sessionExpirationTime, string requesterKey)
    {
        var cookieText = JsonSerializer.Serialize(new AnonInfo
        {
            SessionKey = sessionKey,
            SessionExpirationTime = sessionExpirationTime,
            RequesterKey = requesterKey
        });
        var protectedText = new EncryptedValue(protector, cookieText).Value();
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Path = "/",
            SameSite = SameSiteMode.Lax,
            Domain = string.IsNullOrWhiteSpace(anonOptions.CookieDomain) ? 
                null : 
                anonOptions.CookieDomain
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append(anonOptions.CookieName, protectedText, options);
        SessionKey = sessionKey;
        SessionExpirationTime = sessionExpirationTime;
        RequesterKey = requesterKey;
    }

    private class AnonInfo
    {
        public string SessionKey { get; set; } = "";
        public DateTimeOffset SessionExpirationTime { get; set; }
        public string RequesterKey { get; set; } = "";
    }
}