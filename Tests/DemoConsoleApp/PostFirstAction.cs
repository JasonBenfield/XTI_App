using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostFirstAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("PostStop Starting First Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PostStop First Action Complete");
        return new EmptyActionResult();
    }
}