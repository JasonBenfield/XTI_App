using Microsoft.Extensions.Caching.Memory;

namespace XTI_WebAppClient;

public sealed class XtiTokenAccessor
{
    private readonly IMemoryCache cache;
    private readonly string identifier;
    private readonly Dictionary<Type, Func<IXtiToken>> tokens;
    private readonly DefaultTokenTypeAccessor defaultTokenTypeAccessor;
    private Type? currentTokenType = null;

    internal XtiTokenAccessor(IMemoryCache cache, Dictionary<Type, Func<IXtiToken>> tokens, DefaultTokenTypeAccessor defaultTokenTypeAccessor, string identifier = "default")
    {
        this.cache = cache;
        this.tokens = tokens;
        this.defaultTokenTypeAccessor = defaultTokenTypeAccessor;
        this.identifier = identifier;
    }

    internal void UseToken(Type tokenType)
    {
        currentTokenType = tokenType;
    }

    public void UseToken<T>()
        where T : IXtiToken
    {
        currentTokenType = typeof(T);
    }

    public void Reset()
    {
        var tokenType = GetCurrentTokenType();
        if (tokenType != null)
        {
            cache.Set($"{tokenType.FullName}_{identifier}_token", "");
            cache.Set($"{tokenType.FullName}_{identifier}_userName", "");
        }
    }

    public Task<string> UserName()
    {
        var tokenType = GetCurrentTokenType();
        if (tokenType == null) { throw new ArgumentNullException(nameof(tokenType)); }
        var cacheKey = $"{tokenType.FullName}_{identifier}_userName";
        if (!cache.TryGetValue<string>(cacheKey, out var userName))
        {
            userName = "";
        }
        if (string.IsNullOrWhiteSpace(userName))
        {
            cache.Set(cacheKey, userName);
        }
        return Task.FromResult(userName ?? "");
    }

    public async Task<string> Value()
    {
        var tokenType = GetCurrentTokenType();
        if (tokenType == null) { throw new ArgumentNullException(nameof(tokenType)); }
        var cacheKey = $"{tokenType.FullName}_{identifier}_token";
        if (!cache.TryGetValue<string>(cacheKey, out var tokenValue))
        {
            tokenValue = "";
        }
        if (string.IsNullOrWhiteSpace(tokenValue))
        {
            if (!tokens.TryGetValue(tokenType, out var createToken))
            {
                throw new NotSupportedException($"Token not found for type '{tokenType}'");
            }
            var token = createToken();
            tokenValue = await token.Value();
            cache.Set(cacheKey, tokenValue, TimeSpan.FromDays(1));
        }
        return tokenValue;
    }

    private Type? GetCurrentTokenType() => currentTokenType ?? defaultTokenTypeAccessor.Value;
}