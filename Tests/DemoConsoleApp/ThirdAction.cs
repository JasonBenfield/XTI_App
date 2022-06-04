using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class ThirdAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting Third Action");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("Third Action Complete");
        return new EmptyActionResult();
    }
}