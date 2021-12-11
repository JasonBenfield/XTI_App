namespace XTI_App.Hosting;

public interface IWorker
{
    bool HasStopped { get; }

    Task StopAsync(CancellationToken cancellationToken);
}