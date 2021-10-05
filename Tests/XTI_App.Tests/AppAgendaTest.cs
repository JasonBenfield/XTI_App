using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XTI_App.Fakes;
using XTI_App.Hosting;
using XTI_Core;
using XTI_Core.Fakes;
using XTI_Schedule;

namespace XTI_App.Tests
{
    sealed class AppAgendaTest
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
            var agenda = services.GetService<AppAgenda>();
            var stoppingToken = new CancellationTokenSource();
            await agenda.Start(stoppingToken.Token);
            while (agenda.IsRunning())
            {
                await Task.Delay(100);
            }
            var counter = services.GetService<FirstAgendaItemCounter>();
            Assert.That(counter.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldRunImmediateFromOptions()
        {
            var services = setup
            (
                (sp, agenda) => { },
                KeyValuePair.Create("AppAgenda:ImmediateItems:0:GroupName", "Agenda"),
                KeyValuePair.Create("AppAgenda:ImmediateItems:0:ActionName", "FirstAgendaItem")
            );
            await services.Setup();
            var agenda = services.GetService<AppAgenda>();
            var stoppingToken = new CancellationTokenSource();
            await agenda.Start(stoppingToken.Token);
            while (agenda.IsRunning())
            {
                await Task.Delay(100);
            }
            var counter = services.GetService<FirstAgendaItemCounter>();
            Assert.That(counter.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldNotRunImmediate_WhenIsDisabled()
        {
            var services = setup
            (
                (sp, agenda) => { },
                KeyValuePair.Create("AppAgenda:ImmediateItems:0:GroupName", "Agenda"),
                KeyValuePair.Create("AppAgenda:ImmediateItems:0:ActionName", "FirstAgendaItem"),
                KeyValuePair.Create("AppAgenda:ImmediateItems:0:IsDisabled", "true")
            );
            await services.Setup();
            var agenda = services.GetService<AppAgenda>();
            var stoppingToken = new CancellationTokenSource();
            await agenda.Start(stoppingToken.Token);
            while (agenda.IsRunning())
            {
                await Task.Delay(100);
            }
            var counter = services.GetService<FirstAgendaItemCounter>();
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
            var clock = (FakeClock)services.GetService<Clock>();
            clock.Set(new DateTimeOffset(new DateTime(2021, 10, 4, 10, 30, 0)));
            var agenda = services.GetService<AppAgenda>();
            var stoppingToken = new CancellationTokenSource();
            await agenda.Start(stoppingToken.Token);
            await Task.Delay(TimeSpan.FromSeconds(5));
            stoppingToken.Cancel();
            var counter = services.GetService<FirstAgendaItemCounter>();
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
            var clock = (FakeClock)services.GetService<Clock>();
            clock.Set(new DateTimeOffset(new DateTime(2021, 10, 5, 10, 30, 0)));
            var agenda = services.GetService<AppAgenda>();
            var stoppingToken = new CancellationTokenSource();
            await agenda.Start(stoppingToken.Token);
            await Task.Delay(TimeSpan.FromSeconds(5));
            stoppingToken.Cancel();
            var counter = services.GetService<FirstAgendaItemCounter>();
            Assert.That(counter.Value, Is.EqualTo(0));
        }

        private static IServiceProvider setup(Action<IServiceProvider, AppAgendaBuilder> build, params KeyValuePair<string, string>[] options)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config) =>
                    {
                        config.Sources.Clear();
                        config.AddInMemoryCollection(options ?? new KeyValuePair<string, string>[] { });
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddServicesForTests(hostContext.Configuration);
                        services.AddAppAgenda
                        (
                            hostContext.Configuration,
                            build
                        );
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            return scope.ServiceProvider;
        }
    }
}
