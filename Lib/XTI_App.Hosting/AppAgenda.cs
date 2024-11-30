using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

public sealed class AppAgenda
{
    private readonly IServiceScope scope;
    private readonly ImmediateAppAgendaItem[] preStartItems;
    private readonly AppAgendaItem[] items;
    private readonly ImmediateAppAgendaItem[] postStopItems;
    private readonly List<IWorker> workers = new();
    private SessionWorker? sessionWorker;
    private bool isCurrentVersion;

    internal AppAgenda
    (
        IServiceProvider sp,
        ImmediateAppAgendaItem[] preStartItems,
        AppAgendaItem[] items,
        ImmediateAppAgendaItem[] postStopItems
    )
    {
        scope = sp.CreateScope();
        this.preStartItems = preStartItems;
        this.items = items;
        this.postStopItems = postStopItems;
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        var xtiBasePath = scope.ServiceProvider.GetRequiredService<XtiBasePath>();
        isCurrentVersion = xtiBasePath.VersionKey.Equals(AppVersionKey.Current);
        if (isCurrentVersion)
        {
            var currentSession = scope.ServiceProvider.GetRequiredService<CurrentSession>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var factory = scope.ServiceProvider.GetRequiredService<IActionRunnerFactory>();
            var tempLogRepo = scope.ServiceProvider.GetRequiredService<TempLogRepository>();
            var tempLogSession = factory.CreateTempLogSession();
            await tempLogSession.StartSession();
            sessionWorker = new SessionWorker(currentSession, clock, tempLogSession, tempLogRepo);
            var _ = sessionWorker.StartAsync(stoppingToken);
            var preStartWorker = new ImmediateActionWorker
            (
                scope.ServiceProvider,
                preStartItems
            );
            await preStartWorker.StartAsync(stoppingToken);
            while (!preStartWorker.HasStopped)
            {
                await Task.Delay(100);
            }
            StartWorkers(scope.ServiceProvider, stoppingToken);
        }
    }

    private void StartWorkers(IServiceProvider sp, CancellationToken stoppingToken)
    {
        var immediateWorker = new ImmediateActionWorker
        (
            sp,
            items.OfType<ImmediateAppAgendaItem>().ToArray()
        );
        immediateWorker.StartAsync(stoppingToken);
        workers.Add(immediateWorker);
        var scheduledAppAgendaItems = items.OfType<ScheduledAppAgendaItem>().ToArray();
        foreach (var scheduledItem in scheduledAppAgendaItems.Where(item => item.IsEnabled))
        {
            var worker = new ScheduledActionWorker(sp, scheduledItem);
            worker.StartAsync(stoppingToken);
            workers.Add(worker);
        }
    }

    public bool IsRunning() => workers.Any(w => !w.HasStopped);

    public async Task Stop(CancellationToken stoppingToken)
    {
        if (isCurrentVersion)
        {
            var tempLogRepo = scope.ServiceProvider.GetRequiredService<TempLogRepository>();
            foreach (var worker in workers)
            {
                try
                {
                    await worker.StopAsync(stoppingToken);
                }
                catch (TaskCanceledException) { }
            }
            var timeout = DateTime.UtcNow.AddMinutes(5);
            while
            (
                IsRunning() &&
                !stoppingToken.IsCancellationRequested &&
                DateTime.UtcNow < timeout
            )
            {
                try
                {
                    await Task.Delay(100, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }
            var postStopWorker = new ImmediateActionWorker
            (
                scope.ServiceProvider,
                postStopItems
            );
            await postStopWorker.StartAsync(stoppingToken);
            while (!postStopWorker.HasStopped)
            {
                await Task.Delay(100);
            }
            if(sessionWorker != null)
            {
                await sessionWorker.StopAsync(stoppingToken);
            }
            await tempLogRepo.WriteToLocalStorage();
        }
    }

}