using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XTI_App.Fakes;
using XTI_App.Hosting;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Core.Fakes;
using XTI_Schedule;

namespace XTI_App.Tests;

internal sealed class AppAgendaTest
{
    [Test]
    public async Task ShouldRunImmediate()
    {
        var services = setup
        (
            (sp, agenda) =>
            {
                agenda.AddImmediate<FakeAppApi>(api => api.Agenda.FirstAgendaItem);
            }
        );
        await services.Setup();
        var agenda = services.GetRequiredService<AppAgenda>();
        var stoppingToken = new CancellationTokenSource();
        await agenda.Start(stoppingToken.Token);
        while (agenda.IsRunning())
        {
            await Task.Delay(100);
        }
        var counter = services.GetRequiredService<FirstAgendaItemCounter>();
        Assert.That(counter.Value, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldRunImmediateFromOptions()
    {
        var services = setup
        (
            (sp, agenda) => { },
            KeyValuePair.Create("AppAgenda:ImmediateItems:0:GroupName", (string?)"Agenda"),
            KeyValuePair.Create("AppAgenda:ImmediateItems:0:ActionName", (string?)"FirstAgendaItem")
        );
        await services.Setup();
        var agenda = services.GetRequiredService<AppAgenda>();
        var stoppingToken = new CancellationTokenSource();
        await agenda.Start(stoppingToken.Token);
        while (agenda.IsRunning())
        {
            await Task.Delay(100);
        }
        var counter = services.GetRequiredService<FirstAgendaItemCounter>();
        Assert.That(counter.Value, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldNotRunImmediate_WhenIsDisabled()
    {
        var services = setup
        (
            (sp, agenda) => { },
            KeyValuePair.Create("AppAgenda:ImmediateItems:0:GroupName", (string?)"Agenda"),
            KeyValuePair.Create("AppAgenda:ImmediateItems:0:ActionName", (string?)"FirstAgendaItem"),
            KeyValuePair.Create("AppAgenda:ImmediateItems:0:IsDisabled", (string?)"true")
        );
        await services.Setup();
        var agenda = services.GetRequiredService<AppAgenda>();
        var stoppingToken = new CancellationTokenSource();
        await agenda.Start(stoppingToken.Token);
        while (agenda.IsRunning())
        {
            await Task.Delay(100);
        }
        var counter = services.GetRequiredService<FirstAgendaItemCounter>();
        Assert.That(counter.Value, Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldRunScheduled()
    {
        var services = setup
        (
            (sp, agenda) =>
            {
                agenda.AddScheduled<FakeAppApi>
                (
                    (api, scheduled) => scheduled
                        .Action(api.Agenda.FirstAgendaItem.Path)
                        .AddSchedule
                        (
                            Schedule.On(DayOfWeek.Monday).At(TimeRange.From(10, 0).ForOneHour())
                        )
                        .Interval(TimeSpan.FromSeconds(1))
                );
            }
        );
        await services.Setup();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Set(new DateTimeOffset(new DateTime(2021, 10, 4, 10, 30, 0)));
        var agenda = services.GetRequiredService<AppAgenda>();
        await agenda.Start(new CancellationTokenSource().Token);
        await Task.Delay(TimeSpan.FromSeconds(5));
        await agenda.Stop(new CancellationTokenSource().Token);
        var counter = services.GetRequiredService<FirstAgendaItemCounter>();
        Console.WriteLine($"Counter: {counter.Value}");
        Assert.That(counter.Value, Is.GreaterThan(1));
    }

    [Test]
    public async Task ShouldNotRun_WhenNotScheduled()
    {
        var services = setup
        (
            (sp, agenda) =>
            {
                agenda.AddScheduled<FakeAppApi>
                (
                    (api, scheduled) => scheduled
                        .Action(api.Agenda.FirstAgendaItem.Path)
                        .AddSchedule
                        (
                            Schedule.On(DayOfWeek.Monday).At(TimeRange.From(10, 0).ForOneHour())
                        )
                        .Interval(TimeSpan.FromSeconds(1))
                );
            }
        );
        await services.Setup();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Set(new DateTimeOffset(new DateTime(2021, 10, 5, 10, 30, 0)));
        var agenda = services.GetRequiredService<AppAgenda>();
        var stoppingToken = new CancellationTokenSource();
        await agenda.Start(stoppingToken.Token);
        await Task.Delay(TimeSpan.FromSeconds(5));
        stoppingToken.Cancel();
        var counter = services.GetRequiredService<FirstAgendaItemCounter>();
        Assert.That(counter.Value, Is.EqualTo(0));
    }

    private static IServiceProvider setup(Action<IServiceProvider, AppAgendaBuilder> build, params KeyValuePair<string, string?>[] options)
    {
        var hostBuilder = new XtiHostBuilder();
        hostBuilder.Configuration.AddInMemoryCollection(options ?? new KeyValuePair<string, string?>[0]);
        hostBuilder.Services.AddServicesForTests();
        hostBuilder.Services.AddAppAgenda(build);
        var sp = hostBuilder.Build().Scope();
        return sp;
    }
}