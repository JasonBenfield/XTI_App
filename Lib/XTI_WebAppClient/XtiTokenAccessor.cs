using Microsoft.Extensions.Caching.Memory;

namespace XTI_WebAppClient;

public sealed class XtiTokenAccessor
{
    private readonly IMemoryCache cache;
    private readonly string identifier;
    private readonly Dictionary<Type, Func<IXtiToken>> tokens = new();
    private Type? currentTokenType = null;

    public XtiTokenAccessor(IMemoryCache cache, string identifier = "default")
    {
        this.cache = cache;
        this.identifier = identifier;
    }

    public XtiTokenAccessor AddToken<T>(Func<T> createToken)
        where T : IXtiToken
    {
        tokens.Add(typeof(T), () => createIXtiToken(createToken));
        return this;
    }

    private static IXtiToken createIXtiToken<T>(Func<T> createToken)
        where T : IXtiToken
    {
        return createToken();
    }

    public void UseToken<T>()
        where T : IXtiToken
    {
        currentTokenType = typeof(T);
    }

    public void Reset()
    {
        if (currentTokenType != null)
        {
            cache.Set($"{currentTokenType.FullName}_{identifier}_token", "");
            cache.Set($"{currentTokenType.FullName}_{identifier}_userName", "");
        }
    }

    public Task<string> UserName()
    {
        if (currentTokenType == null) { throw new ArgumentNullException(nameof(currentTokenType)); }
        var cacheKey = $"{currentTokenType.FullName}_{identifier}_userName";
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
        if (currentTokenType == null) { throw new ArgumentNullException(nameof(currentTokenType)); }
        var cacheKey = $"{currentTokenType.FullName}_{identifier}_token";
        if (!cache.TryGetValue<string>(cacheKey, out var tokenValue))
        {
            tokenValue = "";
        }
        if (string.IsNullOrWhiteSpace(tokenValue))
        {
            if (!tokens.TryGetValue(currentTokenType, out var createToken))
            {
                throw new NotSupportedException($"Token not found for type '{currentTokenType}'");
            }
            var token = createToken();
            tokenValue = await token.Value();
            cache.Set(cacheKey, tokenValue);
        }
        return tokenValue;
    }
}