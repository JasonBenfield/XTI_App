using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostThirdAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("PostStop Starting Third Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PostStop Third Action Complete");
        return new EmptyActionResult();
    }
}