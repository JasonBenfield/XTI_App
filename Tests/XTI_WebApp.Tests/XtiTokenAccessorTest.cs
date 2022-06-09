using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_Core.Extensions;
using XTI_WebAppClient;

namespace XTI_WebApp.Tests;

internal sealed class XtiTokenAccessorTest
{
    [Test]
    public async Task ShouldGetToken()
    {
        var sp = setup();
        var xtiTokenAccessor = sp.GetRequiredService<XtiTokenAccessor>();
        xtiTokenAccessor.AddToken(() => new FakeXtiToken1("TokenValue"));
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        var token = await xtiTokenAccessor.Value();
        Assert.That(token, Is.EqualTo("TokenValue"), "Should get token");
    }

    [Test]
    public async Task ShouldCacheToken()
    {
        var sp = setup();
        var xtiTokenAccessor = sp.GetRequiredService<XtiTokenAccessor>();
        var fakeToken = new FakeXtiToken1("TokenValue");
        xtiTokenAccessor.AddToken(() => fakeToken);
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        await xtiTokenAccessor.Value();
        fakeToken.SetValue("ChangedTokenValue");
        var cachedToken = await xtiTokenAccessor.Value();
        Assert.That(cachedToken, Is.EqualTo("TokenValue"), "Should get cached token");
    }

    [Test]
    public async Task ShouldCacheTokenUntilReset()
    {
        var sp = setup();
        var xtiTokenAccessor = sp.GetRequiredService<XtiTokenAccessor>();
        var fakeToken = new FakeXtiToken1("TokenValue");
        xtiTokenAccessor.AddToken(() => fakeToken);
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        await xtiTokenAccessor.Value();
        fakeToken.SetValue("ChangedTokenValue");
        xtiTokenAccessor.Reset();
        var cachedToken = await xtiTokenAccessor.Value();
        Assert.That(cachedToken, Is.EqualTo("ChangedTokenValue"), "Should get cached token until reset");
    }

    [Test]
    public async Task ShouldUseCurrentToken()
    {
        var sp = setup();
        var xtiTokenAccessor = sp.GetRequiredService<XtiTokenAccessor>();
        xtiTokenAccessor.AddToken(() => new FakeXtiToken1("Token1"));
        xtiTokenAccessor.AddToken(() => new FakeXtiToken2("Token2"));
        xtiTokenAccessor.UseToken<FakeXtiToken1>();
        var token = await xtiTokenAccessor.Value();
        Assert.That(token, Is.EqualTo("Token1"), "Should use current token");
        xtiTokenAccessor.UseToken<FakeXtiToken2>();
        token = await xtiTokenAccessor.Value();
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

        public Task<string> Value() => Task.FromResult(value);

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

        public Task<string> Value() => Task.FromResult(value);

        public void SetValue(string value) => this.value = value;
    }

    private IServiceProvider setup()
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Services.AddMemoryCache();
        hostBuilder.Services.AddScoped<XtiTokenAccessor>();
        return hostBuilder.Build().Scope();
    }

}
