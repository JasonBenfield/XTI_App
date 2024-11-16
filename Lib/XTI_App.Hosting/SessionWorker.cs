using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

internal sealed class SessionWorker : BackgroundService
{
    private readonly IServiceProvider sp;
    private TempLogSession? tempLogSession;

    public SessionWorker(IServiceProvider sp)
    {
        this.sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentSession = sp.GetRequiredService<CurrentSession>();
        var clock = sp.GetRequiredService<IClock>();
        var timeStarted = clock.Now();
        var factory = sp.GetRequiredService<IActionRunnerFactory>();
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
            }
            catch { }
            try
            {
                var tempLogRepo = sp.GetRequiredService<TempLogRepository>();
                await tempLogRepo.WriteToLocalStorage();
            }
            catch { }
        }
        tempLogSession = null;
    }
}
