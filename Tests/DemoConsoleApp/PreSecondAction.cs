using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreSecondAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("PreStart Starting Second Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PreStart Second Action Complete");
        return new EmptyActionResult();
    }
}