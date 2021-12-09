using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class ThirdAgendaItemAction : AppAction<EmptyRequest, EmptyActionResult>
{
    public static int Counter { get; private set; }

    public Task<EmptyActionResult> Execute(EmptyRequest model)
    {
        Console.WriteLine($"Third Agenda Item: {DateTime.Now:HH:mm:ss}");
        Counter++;
        return Task.FromResult(new EmptyActionResult());
    }
}