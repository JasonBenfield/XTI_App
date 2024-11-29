using Microsoft.Extensions.Hosting;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

internal sealed class SessionWorker : BackgroundService
{
    private readonly CurrentSession currentSession;
    private readonly IClock clock;
    private readonly IActionRunnerFactory factory;
    private readonly TempLogRepository tempLogRepository;
    private TempLogSession? tempLogSession;

    public SessionWorker
    (
        CurrentSession currentSession, 
        IClock clock, 
        IActionRunnerFactory factory,
        TempLogRepository tempLogRepository
    )
    {
        this.currentSession = currentSession;
        this.clock = clock;
        this.factory = factory;
        this.tempLogRepository = tempLogRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timeStarted = clock.Now();
        tempLogSession = factory.CreateTempLogSession();
        await tempLogSession.StartSession();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = clock.Now();
                if (currentTime > timeStarted.Add(TimeSpan.FromHours(12)))
                {
                    await tempLogSession.EndSession();
                    currentSession.SessionKey = "";
                    await tempLogSession.StartSession();
                    timeStarted = clock.Now();
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        var s = tempLogSession;
        if (s != null)
        {
            try
            {
                await s.EndSession();
                try
                {
                    await tempLogRepository.WriteToLocalStorage();
                }
                catch { }
            }
            catch { }
        }
        tempLogSession = null;
    }
}
