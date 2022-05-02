using XTI_App.Api;

namespace DemoConsoleApp;

public sealed class PostSecondAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public async Task<EmptyActionResult> Execute(EmptyRequest model)
    {
        Console.WriteLine("PostStop Starting Second Action");
        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("PostStop Second Action Complete");
        return new EmptyActionResult();
    }
}