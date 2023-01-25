using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using XTI_App.Hosting;
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
        setTimeWithinSchedule(host.Services);
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
        setTimeWithinSchedule(host.Services);
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
        setTimeWithinSchedule(host.Services);
        var _ = Task.Run(() => host.Run());
        await delay();
        var startRequests = await getStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        var startRequest = startRequests.FirstOrDefault(r => api.Test.RunContinuously.Path.Equals(r.Path));
        Assert.That(startRequest, Is.Not.Null, "Should add start request");
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldEndRequest()
    {
        var host = BuildHost().Build();
        setTimeWithinSchedule(host.Services);
        var counter = host.Services.GetService<Counter>();
        var _ = Task.Run(() => host.Run());
        await delay();
        var startRequests = await getStartRequests(host.Services);
        var endRequests = await getEndRequests(host.Services);
        var endRequest = endRequests.FirstOrDefault(r => r.RequestKey == startRequests[0].RequestKey);
        Assert.That(endRequest, Is.Not.Null, "Should end request");
        await host.StopAsync();
    }

    [Test]
    public async Task ShouldLogException()
    {
        var host = BuildHost().Build();
        setTimeWithinSchedule(host.Services);
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = false;
        testOptions.ThrowException = true;
        var counter = host.Services.GetRequiredService<Counter>();
        var _ = Task.Run(() => host.Run());
        await delay();
        var startRequests = await getStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        var startRequest = startRequests.First
        (
            r => api.Test.OptionalRun.Path.Equals(r.Path)
        );
        var logEvents = await getLogEvents(host.Services);
        logEvents = logEvents.Where(evt => evt.RequestKey == startRequest.RequestKey).ToArray();
        Assert.That(logEvents.Count(), Is.GreaterThan(0), "Should log exceptions");
        Assert.That(logEvents[0].Severity, Is.EqualTo(AppEventSeverity.Values.CriticalError), "Should log critical error");
        await host.StopAsync();
    }

    private static void setTimeWithinSchedule(IServiceProvider services)
    {
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Set(new DateTimeOffset(new DateTime(2020, 10, 16, 9, 30, 0)));
    }

    [Test]
    public async Task ShouldNotRunScheduledAction_WhenNotInSchedule()
    {
        var host = BuildHost().Build();
        var clock = (FakeClock)host.Services.GetRequiredService<IClock>();
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
        setTimeWithinSchedule(host.Services);
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var counter = host.Services.GetRequiredService<Counter>();
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = false;
        var _ = Task.Run(() => host.Run());
        await delay();
        fastForward(host.Services, TimeSpan.FromMinutes(1));
        var startRequests = await getStartRequests(host.Services);
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
        setTimeWithinSchedule(host.Services);
        var tempLog = host.Services.GetRequiredService<TempLog>();
        var counter = host.Services.GetRequiredService<Counter>();
        var testOptions = host.Services.GetRequiredService<TestOptions>();
        testOptions.IsOptional = true;
        var _ = Task.Run(() => host.Run());
        await delay();
        fastForward(host.Services, TimeSpan.FromMinutes(1));
        var startRequests = await getStartRequests(host.Services);
        var api = host.Services.GetRequiredService<TestApi>();
        startRequests = startRequests.Where(r => api.Test.OptionalRun.Path.Equals(r.Path)).ToArray();
        Assert.That(startRequests.Count(), Is.EqualTo(0), "Should not start request for an optional action");
        Assert.That(counter.OptionalValue, Is.EqualTo(0), "Should not run an optional action");
        await host.StopAsync();
    }

    private void fastForward(IServiceProvider services, TimeSpan howLong)
    {
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Add(howLong);
    }

    private Task<StartRequestModel[]> getStartRequests(IServiceProvider services)
    {
        var tempLog = services.GetRequiredService<TempLog>();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        var files = tempLog.StartRequestFiles(clock.Now()).ToArray();
        return deserializeLogFiles<StartRequestModel>(files);
    }

    private Task<EndRequestModel[]> getEndRequests(IServiceProvider services)
    {
        var tempLog = services.GetRequiredService<TempLog>();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        return deserializeLogFiles<EndRequestModel>
        (
            tempLog.EndRequestFiles(clock.Now()).ToArray()
        );
    }

    private Task<LogEntryModel[]> getLogEvents(IServiceProvider services)
    {
        var tempLog = services.GetRequiredService<TempLog>();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        return deserializeLogFiles<LogEntryModel>
        (
            tempLog.LogEventFiles(clock.Now()).ToArray()
        );
    }

    private async Task<T[]> deserializeLogFiles<T>(IEnumerable<ITempLogFile> logFiles)
        where T : new()
    {
        var logObjects = new List<T>();
        foreach (var logFile in logFiles)
        {
            var deserialized = await logFile.Read();
            if (!string.IsNullOrWhiteSpace(deserialized))
            {
                var logObject = XtiSerializer.Deserialize<T>(deserialized);
                logObjects.Add(logObject);
            }
        }
        return logObjects.ToArray();
    }

    private static Task delay() => Task.Delay(500);

    private IHostBuilder BuildHost()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
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
}