using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Core;
using XTI_TempLog;
using XTI_TempLog.Abstractions;

namespace XTI_ConsoleApp.Tests;

public sealed class AppMiddlewareImmediateActionTest
{
    [Test]
    public async Task ShouldStartSession()
    {
        var host = await RunService();
        var sessionDetails = await GetSessionDetails(host.Services);
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        Assert.That(sessions.Length, Is.EqualTo(1), "Should start session");
    }

    [Test]
    public async Task ShouldStartRequest()
    {
        var host = await RunService();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var clock = host.Services.GetRequiredService<IClock>();
        var requests = await GetRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        requests = requests.Where(r => api.Test.RunContinuously.Path.Equals(r.Path)).ToArray();
        Assert.That(requests.Length, Is.GreaterThan(0), "Should start request");
    }

    [Test]
    public async Task ShouldEndRequest()
    {
        var host = await RunService();
        var requests = await GetRequests(host.Services);
        requests = requests.Where(r => r.TimeEnded.Year < 9999).ToArray();
        Assert.That(requests.Length, Is.EqualTo(2), "Should end request");
    }

    [Test]
    public async Task ShouldEndSession()
    {
        var host = await RunService();
        var sessionDetails = await GetSessionDetails(host.Services);
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        sessions = sessions.Where(r => r.TimeEnded.Year < 9999).ToArray();
        Assert.That(sessions.Length, Is.EqualTo(1), "Should end session");
    }

    private async Task<IHost> RunService()
    {
        var host = BuildHost().Build();
        return await RunHost(host);
    }

    private static async Task<IHost> RunHost(IHost host)
    {
        var envContext = (FakeAppEnvironmentContext)host.Services.GetRequiredService<IAppEnvironmentContext>();
        envContext.Environment = new AppEnvironment
        (
            "test.user",
            "AppMiddleware",
            "my-computer",
            "Windows 10",
            123
        );
        var _ = Task.Run(() => host.StartAsync());
        var counter = host.Services.GetRequiredService<Counter>();
        while (counter.ContinuousValue == 0)
        {
            await Task.Delay(100);
        }
        await host.StopAsync();
        return host;
    }

    private IHostBuilder BuildHost()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
        return Host.CreateDefaultBuilder([])
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTestServiceAppServices
                (
                    (sp, b) =>
                    {
                        b.AddImmediate<TestApi>(api => api.Test.RunContinuously);
                        b.AddImmediate<TestApi>(api => api.Lifetime.StopApplication);
                    }
                );
            });
    }

    private static async Task<TempLogSessionDetailModel[]> GetSessionDetails(IServiceProvider sp)
    {
        var clock = sp.GetRequiredService<IClock>();
        var tempLog = sp.GetRequiredService<TempLog>();
        var logFiles = tempLog.Files(clock.Now().AddSeconds(1), 100);
        var sessionDetails = new List<TempLogSessionDetailModel>();
        foreach(var logFile in logFiles)
        {
            var fileSessionDetails = await logFile.Read();
            sessionDetails.AddRange(fileSessionDetails);
        }
        return sessionDetails.ToArray();
    }

    private async Task<TempLogRequestModel[]> GetRequests(IServiceProvider sp)
    {
        var sessionDetails = await GetSessionDetails(sp);
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        return requests;
    }

}