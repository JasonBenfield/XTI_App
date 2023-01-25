using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class SecondAgendaItemAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public static int Counter { get; private set; }

    public Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        Console.WriteLine($"Second Agenda Item: {DateTime.Now:HH:mm:ss}");
        Counter++;
        return Task.FromResult(new EmptyActionResult());
    }
}