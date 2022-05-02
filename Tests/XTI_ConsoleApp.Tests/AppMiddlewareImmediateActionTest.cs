using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_TempLog;
using XTI_TempLog.Abstractions;
using XTI_TempLog.Fakes;

namespace XTI_ConsoleApp.Tests;

public sealed class AppMiddlewareImmediateActionTest
{
    [Test]
    public async Task ShouldStartSession()
    {
        var host = await runService();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var clock = host.Services.GetRequiredService<IClock>();
        var startSessionFiles = tempLog.StartSessionFiles(clock.Now());
        Assert.That(startSessionFiles.Count(), Is.EqualTo(1), "Should start session");
    }

    [Test]
    public async Task ShouldStartRequest()
    {
        var host = await runService();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var clock = host.Services.GetRequiredService<IClock>();
        var startRequests = await getStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        startRequests = startRequests.Where(r => api.Test.RunContinuously.Path.Equals(r.Path)).ToArray();
        Assert.That(startRequests.Length, Is.GreaterThan(0), "Should start request");
    }

    private Task<StartRequestModel[]> getStartRequests(IServiceProvider services)
    {
        var tempLog = services.GetRequiredService<TempLog>();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        var files = tempLog.StartRequestFiles(clock.Now()).ToArray();
        return deserializeLogFiles<StartRequestModel>(files);
    }

    private async Task<T[]> deserializeLogFiles<T>(IEnumerable<ITempLogFile> logFiles)
        where T : new()
    {
        var logObjects = new List<T>();
        foreach (var logFile in logFiles)
        {
            var deserialized = await logFile.Read();
            var logObject = XtiSerializer.Deserialize<T>(deserialized);
            logObjects.Add(logObject);
        }
        return logObjects.ToArray();
    }

    [Test]
    public async Task ShouldEndRequest()
    {
        var host = await runService();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var clock = host.Services.GetRequiredService<IClock>();
        var endRequestFiles = tempLog.EndRequestFiles(clock.Now());
        Assert.That(endRequestFiles.Count(), Is.EqualTo(2), "Should end request");
    }

    [Test]
    public async Task ShouldEndSession()
    {
        var host = await runService();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var clock = host.Services.GetRequiredService<IClock>();
        var endSessionFiles = tempLog.EndSessionFiles(clock.Now());
        Assert.That(endSessionFiles.Count(), Is.EqualTo(1), "Should end session");
    }

    private async Task<IHost> runService()
    {
        var host = BuildHost().Build();
        return await runHost(host);
    }

    private static async Task<IHost> runHost(IHost host)
    {
        var envContext = (FakeAppEnvironmentContext)host.Services.GetRequiredService<IAppEnvironmentContext>();
        envContext.Environment = new AppEnvironment
        (
            "test.user",
            "AppMiddleware",
            "my-computer",
            "Windows 10",
            "Service"
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
        return Host.CreateDefaultBuilder(new string[] { })
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
}