using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

public sealed class AppAgenda
{
    private readonly IServiceProvider sp;
    private readonly IClock clock;
    private readonly XtiEnvironment xtiEnv;
    private readonly XtiBasePath xtiBasePath;
    private readonly TempLogRepository tempLogRepo;
    private readonly ImmediateAppAgendaItem[] preStartItems;
    private readonly AppAgendaItem[] items;
    private readonly ImmediateAppAgendaItem[] postStopItems;
    private readonly List<IWorker> workers = new();
    private SessionWorker? sessionWorker;
    private bool hasAgenda;

    internal AppAgenda
    (
        IServiceProvider sp,
        IClock clock,
        XtiEnvironment xtiEnv,
        XtiBasePath xtiBasePath,
        TempLogRepository tempLogRepo,
        ImmediateAppAgendaItem[] preStartItems,
        AppAgendaItem[] items,
        ImmediateAppAgendaItem[] postStopItems
    )
    {
        this.sp = sp;
        this.clock = clock;
        this.xtiEnv = xtiEnv;
        this.xtiBasePath = xtiBasePath;
        this.tempLogRepo = tempLogRepo;
        this.preStartItems = preStartItems;
        this.items = items;
        this.postStopItems = postStopItems;
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        hasAgenda = xtiBasePath.VersionKey.Equals(AppVersionKey.Current) &&
            (preStartItems.Any() || items.Any() || postStopItems.Any());
        if (hasAgenda)
        {
            var currentSession = sp.GetRequiredService<CurrentSession>();
            var factory = sp.GetRequiredService<IActionRunnerFactory>();
            var tempLogSession = factory.CreateTempLogSession();
            await tempLogSession.StartSession();
            sessionWorker = new SessionWorker(currentSession, clock, tempLogSession, tempLogRepo);
            var _ = sessionWorker.StartAsync(stoppingToken);
            var preStartWorker = new ImmediateActionWorker
            (
                sp,
                xtiEnv,
                xtiBasePath,
                preStartItems
            );
            await preStartWorker.StartAsync(stoppingToken);
            while (!preStartWorker.HasStopped)
            {
                await Task.Delay(100, stoppingToken);
            }
            StartWorkers(clock, xtiEnv, xtiBasePath, stoppingToken);
        }
    }

    private void StartWorkers
    (
        IClock clock,
        XtiEnvironment xtiEnv,
        XtiBasePath xtiBasePath,
        CancellationToken stoppingToken
    )
    {
        var immediateWorker = new ImmediateActionWorker
        (
            sp,
            xtiEnv,
            xtiBasePath,
            items.OfType<ImmediateAppAgendaItem>().ToArray()
        );
        immediateWorker.StartAsync(stoppingToken);
        workers.Add(immediateWorker);
        var scheduledAppAgendaItems = items
            .OfType<ScheduledAppAgendaItem>()
            .Where(item => item.IsEnabled)
            .ToArray();
        foreach (var scheduledItem in scheduledAppAgendaItems)
        {
            var worker = new ScheduledActionWorker
            (
                sp,
                clock,
                xtiEnv,
                xtiBasePath,
                scheduledItem
            );
            worker.StartAsync(stoppingToken);
            workers.Add(worker);
        }
    }

    public bool IsRunning() => workers.Any(w => !w.HasStopped);

    public async Task Stop(CancellationToken stoppingToken)
    {
        if (hasAgenda)
        {
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
                sp,
                xtiEnv,
                xtiBasePath,
                postStopItems
            );
            await postStopWorker.StartAsync(stoppingToken);
            while (!postStopWorker.HasStopped)
            {
                await Task.Delay(100);
            }
            if (sessionWorker != null)
            {
                await sessionWorker.StopAsync(stoppingToken);
            }
            await tempLogRepo.WriteToLocalStorage();
        }
    }

}