using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class SecondAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting Second Action");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("Second Action Complete");
        return new EmptyActionResult();
    }
}