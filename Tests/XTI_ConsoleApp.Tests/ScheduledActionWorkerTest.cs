using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Schedule;
using XTI_TempLog;
using XTI_TempLog.Abstractions;

namespace XTI_ConsoleApp.Tests;

public sealed class ScheduledActionWorkerTest
{
    [Test]
    public async Task ShouldRunScheduledAction()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var _ = Task.Run(() => host.Run());
        await delay();
        var counter = host.Services.GetRequiredService<Counter>();
        Assert.That(counter.ContinuousValue, Is.GreaterThan(0));
        Console.WriteLine($"Counter value: {counter.ContinuousValue}");
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldRunOnce_WhenTypeIsPeriodicUntilSuccess()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var _ = Task.Run(() => host.Run());
        await delay();
        var counter = host.Services.GetRequiredService<Counter>();
        Assert.That(counter.UntilSuccessValue, Is.EqualTo(1));
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldStartRequest()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var _ = Task.Run(() => host.Run());
        await delay();
        var startRequests = await GetStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        var startRequest = startRequests.FirstOrDefault(r => api.Test.RunContinuously.Path.Equals(r.Path));
        Assert.That(startRequest, Is.Not.Null, "Should add start request");
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldEndRequest()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var counter = host.Services.GetService<Counter>();
        var _ = Task.Run(() => host.Run());
        await delay();
        var startRequests = await GetStartRequests(host.Services);
        var endRequests = await GetEndRequests(host.Services);
        var endRequest = endRequests.FirstOrDefault(r => r.RequestKey == startRequests[0].RequestKey);
        Assert.That(endRequest, Is.Not.Null, "Should end request");
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldLogException()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = false;
        testOptions.ThrowException = true;
        var counter = host.Services.GetRequiredService<Counter>();
        var _ = Task.Run(() => host.Run());
        await delay();
        var requests = await GetStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        var request = requests.First
        (
            r => api.Test.OptionalRun.Path.Equals(r.Path)
        );
        var logEvents = await GetLogEntries(host.Services, request);
        Assert.That(logEvents.Count(), Is.GreaterThan(0), "Should log exceptions");
        Assert.That(logEvents[0].Severity, Is.EqualTo(AppEventSeverity.Values.CriticalError), "Should log critical error");
        await host.StopAsync();
    }

    private static void SetTimeWithinSchedule(IServiceProvider services)
    {
        var clock = services.GetRequiredService<FakeClock>();
        clock.Set(new DateTimeOffset(new DateTime(2020, 10, 16, 9, 30, 0)));
    }

    [Test]
    public async Task ShouldNotRunScheduledAction_WhenNotInSchedule()
    {
        var host = BuildHost().Build();
        var clock = host.Services.GetRequiredService<FakeClock>();
        clock.Set(new DateTime(2020, 10, 16, 14, 30, 0, DateTimeKind.Utc));
        var _ = Task.Run(() => host.RunAsync());
        await delay();
        var counter = host.Services.GetRequiredService<Counter>();
        Assert.That(counter.ContinuousValue, Is.EqualTo(0));
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldRunOptionalAction_WhenItIsNotOptional()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var counter = host.Services.GetRequiredService<Counter>();
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = false;
        var _ = Task.Run(() => host.Run());
        await delay();
        FastForward(host.Services, TimeSpan.FromMinutes(1));
        var startRequests = await GetStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        startRequests = startRequests.Where(r => api.Test.OptionalRun.Path.Equals(r.Path)).ToArray();
        Assert.That(startRequests.Count(), Is.GreaterThan(0), "Should start request when action is not optional");
        Assert.That(counter.OptionalValue, Is.GreaterThan(0));
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldNotRunOptionalAction_WhenItIsOptional()
    {
        var host = BuildHost().Build();
        SetTimeWithinSchedule(host.Services);
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var counter = host.Services.GetRequiredService<Counter>();
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = true;
        var _ = Task.Run(() => host.Run());
        await delay();
        FastForward(host.Services, TimeSpan.FromMinutes(1));
        var startRequests = await GetStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        startRequests = startRequests.Where(r => api.Test.OptionalRun.Path.Equals(r.Path)).ToArray();
        Assert.That(startRequests.Count(), Is.EqualTo(0), "Should not start request for an optional action");
        Assert.That(counter.OptionalValue, Is.EqualTo(0), "Should not run an optional action");
        await host.StopAsync();
    }

    private void FastForward(IServiceProvider services, TimeSpan howLong)
    {
        var clock = services.GetRequiredService<FakeClock>();
        clock.Add(howLong);
    }

    private async Task<TempLogRequestModel[]> GetStartRequests(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        return requests;
    }

    private async Task<TempLogRequestModel[]> GetEndRequests(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .Where(r => r.TimeEnded.Year < 9999)
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        return requests;
    }

    private async Task<LogEntryModel[]> GetLogEntries(IServiceProvider services, TempLogRequestModel request)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var logEntries = sessionDetails
            .SelectMany
            (
                sd => 
                    sd.RequestDetails.Where(rd => rd.Request.RequestKey == request.RequestKey)
                        .SelectMany(rd => rd.LogEntries)
            )
            .OrderBy(r => r.TimeOccurred)
            .ToArray();
        return logEntries;
    }

    private static Task delay() => Task.Delay(500);

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
                        b.AddScheduled<TestApi>
                        (
                            (api, sched) =>
                            {
                                sched
                                    .Action(api.Test.RunContinuously.Path)
                                    .RunContinuously()
                                    .Interval(TimeSpan.FromMilliseconds(100))
                                    .AddSchedule
                                    (
                                        Schedule.On(DayOfWeek.Friday).At(TimeRange.From(new TimeOnly(9, 0)).ForOneHour())
                                    );
                            }
                        );
                        b.AddScheduled<TestApi>
                        (
                            (api, sched) =>
                            {
                                sched
                                    .Action(api.Test.OptionalRun.Path)
                                    .RunContinuously()
                                    .Interval(TimeSpan.FromMilliseconds(100))
                                    .AddSchedule
                                    (
                                        Schedule.On(DayOfWeek.Friday).At(TimeRange.From(new TimeOnly(9, 0)).ForOneHour())
                                    );
                            }
                        );
                        b.AddScheduled<TestApi>
                        (
                            (api, sched) =>
                            {
                                sched
                                    .Action(api.Test.RunUntilSuccess.Path)
                                    .RunUntilSuccess()
                                    .Interval(TimeSpan.FromMilliseconds(100))
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