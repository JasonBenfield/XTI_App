using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreFirstAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("PreStart Starting First Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PreStart First Action Complete");
        return new EmptyActionResult();
    }
}