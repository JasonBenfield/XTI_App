using Microsoft.Extensions.Caching.Memory;

namespace XTI_WebAppClient;

public sealed class XtiTokenAccessorFactory
{
    private readonly IMemoryCache cache;
    private readonly Dictionary<Type, Func<IXtiToken>> tokens = new();
    private DefaultTokenTypeAccessor defaultTokenTypeAccessor;
    private string defaultIdentifier = "default";

    public XtiTokenAccessorFactory(IMemoryCache cache)
    {
        this.cache = cache;
        defaultTokenTypeAccessor = new();
    }

    public XtiTokenAccessorFactory AddToken<T>(Func<T> createToken)
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

    public void UseDefaultToken<T>()
        where T : IXtiToken
    {
        defaultTokenTypeAccessor.Value = typeof(T);
    }

    public void SetDefaultIdentifier(string defaultIdentifier)
    {
        this.defaultIdentifier = defaultIdentifier;
    }

    public XtiTokenAccessor Create() => Create(defaultIdentifier);

    public XtiTokenAccessor Create(string identifier) =>
        new XtiTokenAccessor(cache, tokens, defaultTokenTypeAccessor, identifier);
}
