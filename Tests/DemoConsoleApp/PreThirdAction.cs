using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PreThirdAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine("PreStart Starting Third Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PreStart Third Action Complete");
        return new EmptyActionResult();
    }
}