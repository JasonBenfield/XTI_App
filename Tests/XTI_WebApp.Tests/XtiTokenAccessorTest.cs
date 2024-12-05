using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_Core.Extensions;
using XTI_WebAppClient;

namespace XTI_WebApp.Tests;

internal sealed class XtiTokenAccessorTest
{
    [Test]
    public async Task ShouldGetToken()
    {
        var sp = setup();
        var xtiTokenAccessorFactory = sp.GetRequiredService<XtiTokenAccessorFactory>();
        xtiTokenAccessorFactory.AddToken(() => new FakeXtiToken1("TokenValue"));
        var xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        var token = await xtiTokenAccessor.Value(default);
        Assert.That(token, Is.EqualTo("TokenValue"), "Should get token");
    }

    [Test]
    public async Task ShouldCacheToken()
    {
        var sp = setup();
        var xtiTokenAccessorFactory = sp.GetRequiredService<XtiTokenAccessorFactory>();
        var fakeToken = new FakeXtiToken1("TokenValue");
        xtiTokenAccessorFactory.AddToken(() => fakeToken);
        var xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        await xtiTokenAccessor.Value(default);
        fakeToken.SetValue("ChangedTokenValue");
        var cachedToken = await xtiTokenAccessor.Value(default);
        Assert.That(cachedToken, Is.EqualTo("TokenValue"), "Should get cached token");
    }

    [Test]
    public async Task ShouldCacheTokenUntilReset()
    {
        var sp = setup();
        var xtiTokenAccessorFactory = sp.GetRequiredService<XtiTokenAccessorFactory>();
        var fakeToken = new FakeXtiToken1("TokenValue");
        xtiTokenAccessorFactory.AddToken(() => fakeToken);
        var xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        await xtiTokenAccessor.Value(default);
        fakeToken.SetValue("ChangedTokenValue");
        xtiTokenAccessor.Reset();
        var cachedToken = await xtiTokenAccessor.Value(default);
        Assert.That(cachedToken, Is.EqualTo("ChangedTokenValue"), "Should get cached token until reset");
    }

    [Test]
    public async Task ShouldUseCurrentToken()
    {
        var sp = setup();
        var xtiTokenAccessorFactory = sp.GetRequiredService<XtiTokenAccessorFactory>();
        xtiTokenAccessorFactory.AddToken(() => new FakeXtiToken1("Token1"));
        xtiTokenAccessorFactory.AddToken(() => new FakeXtiToken2("Token2"));
        var xtiTokenAccessor = xtiTokenAccessorFactory.Create();
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        var token = await xtiTokenAccessor.Value(default);
        Assert.That(token, Is.EqualTo("Token1"), "Should use current token");
        xtiTokenAccessor.UseToken<FakeXtiToken2>();
        token = await xtiTokenAccessor.Value(default);
        Assert.That(token, Is.EqualTo("Token2"), "Should use current token");
    }

    private sealed class FakeXtiToken1 : IXtiToken
    {
        private string value;

        public FakeXtiToken1(string value)
        {
            this.value = value;
        }

        public void Reset()
        {
        }

        public Task<string> UserName() => Task.FromResult("Fake.user1");

        public Task<string> Value(CancellationToken ct) => Task.FromResult(value);

        public void SetValue(string value) => this.value = value;
    }

    private sealed class FakeXtiToken2 : IXtiToken
    {
        private string value;

        public FakeXtiToken2(string value)
        {
            this.value = value;
        }

        public void Reset()
        {
        }

        public Task<string> UserName() => Task.FromResult("Fake.user2");

        public Task<string> Value(CancellationToken ct) => Task.FromResult(value);

        public void SetValue(string value) => this.value = value;
    }

    private IServiceProvider setup()
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddMemoryCache();
        hostBuilder.Services.AddScoped<XtiTokenAccessorFactory>();
        return hostBuilder.Build().Scope();
    }

}
