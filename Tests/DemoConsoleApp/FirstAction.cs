using XTI_App.Abstractions;
using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class FirstAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting First Action");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("First Action Complete");
        return new EmptyActionResult();
    }
}