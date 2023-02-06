using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Schedule;
using XTI_TempLog;
using XTI_TempLog.Fakes;

namespace XTI_ConsoleApp.Tests;

public sealed class AppMiddlewareScheduledActionTest
{
    [Test]
    public async Task ShouldLogSessionsAndRequests()
    {
        var host = await runService();
        var clock = host.Services.GetRequiredService<IClock>();
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var startSessionFiles = tempLog.StartSessionFiles(clock.Now());
        Assert.That(startSessionFiles.Count(), Is.GreaterThanOrEqualTo(1), "Should start session");
        var startRequestFiles = tempLog.StartRequestFiles(clock.Now());
        Assert.That(startRequestFiles.Count(), Is.GreaterThanOrEqualTo(1), "Should start request");
        var endRequestFiles = tempLog.EndRequestFiles(clock.Now());
        Assert.That(endRequestFiles.Count(), Is.GreaterThanOrEqualTo(1), "Should end request");
        var endSessionFiles = tempLog.EndSessionFiles(clock.Now());
        Assert.That(endSessionFiles.Count(), Is.GreaterThanOrEqualTo(1), "Should end session");
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
            123
        );
        var clock = (FakeClock)host.Services.GetRequiredService<IClock>();
        clock.Set(new DateTime(2020, 10, 16, 9, 30, 0));
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
        return Host.CreateDefaultBuilder(new string[0])
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTestServiceAppServices
                (
                    (sp, b) =>
                    {
                        b.AddScheduled<TestApi>
                        (
                            (api, sched) =>
                            {
                                sched
                                    .Action(api.Test.RunContinuously.Path)
                                    .RunContinuously()
                                    .Interval(TimeSpan.FromMilliseconds(500))
                                    .AddSchedule
                                    (
                                        Schedule.On(DayOfWeek.Friday).At(TimeRange.From(new TimeOnly(9, 0)).ForOneHour())
                                    );
                            }
                        );
                    }
                );
            });
    }
}