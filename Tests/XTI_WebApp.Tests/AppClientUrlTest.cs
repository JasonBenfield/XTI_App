using NUnit.Framework;
using XTI_WebApp.Abstractions;
using XTI_WebAppClient;

namespace XTI_WebApp.Tests;

internal sealed class AppClientUrlTest
{
    [Test]
    public async Task ShouldGetUrl()
    {
        var clientUrl = new AppClientUrl(new FakeAppClientDomain()).WithApp("Fake", "Current");
        var url = await clientUrl.WithGroup("Group1").Value("Action1", "");
        Assert.That(url, Is.EqualTo("https://webapps.example.com/Fake/Current/Group1/Action1"));
    }

    private sealed class FakeAppClientDomain : IAppClientDomain
    {
        public Task<string> Value(string appName, string version) =>
            Task.FromResult("webapps.example.com");
    }
}