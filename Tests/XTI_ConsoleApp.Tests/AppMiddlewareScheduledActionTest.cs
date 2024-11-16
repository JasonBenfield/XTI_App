using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Schedule;
using XTI_TempLog;
using XTI_TempLog.Abstractions;

namespace XTI_ConsoleApp.Tests;

public sealed class AppMiddlewareScheduledActionTest
{
    [Test]
    public async Task ShouldLogSessionsAndRequests()
    {
        var host = await RunService();
        var clock = host.Services.GetRequiredService<IClock>();
        var logFiles = await WriteLogFiles(host.Services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        Assert.That(sessions.Length, Is.GreaterThanOrEqualTo(1), "Should start session");
        Assert.That(requests.Length, Is.GreaterThanOrEqualTo(1), "Should start request");
        var endedRequests = requests.Where(r => r.TimeEnded.Year < 9999).ToArray();
        Assert.That(endedRequests.Length, Is.GreaterThanOrEqualTo(1), "Should end request");
        var endedSessions = sessions.Where(s => s.TimeEnded.Year < 9999).ToArray();
        Assert.That(endedSessions.Length, Is.GreaterThanOrEqualTo(1), "Should end session");
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
        var clock = (FakeClock)host.Services.GetRequiredService<IClock>();
        clock.Set(new DateTime(2020, 10, 16, 9, 30, 0));
        var _ = Task.Run(() => host.StartAsync());
        var counter = host.Services.GetRequiredService<Counter>();
        var timeStarted = DateTimeOffset.UtcNow;
        while (counter.ContinuousValue == 0)
        {
            var timeElapsed = DateTimeOffset.UtcNow - timeStarted;
            if (timeElapsed.TotalSeconds > 5)
            {
                throw new Exception("Timed out");
            }
            await Task.Delay(100);
        }
        await host.StopAsync();
        return host;
    }

    private IHostBuilder BuildHost()
    {
        return Host.CreateDefaultBuilder([])
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
                                    .Action(api.Test.RunContinuously)
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

    private static async Task<ITempLogFile[]> WriteLogFiles(IServiceProvider sp)
    {
        var tempLogRepo = sp.GetRequiredService<TempLogRepository>();
        await tempLogRepo.WriteToLocalStorage();
        var clock = sp.GetRequiredService<IClock>();
        var tempLog = sp.GetRequiredService<TempLog>();
        var logFiles = tempLog.Files(clock.Now().AddSeconds(1), 100);
        return logFiles;
    }
}