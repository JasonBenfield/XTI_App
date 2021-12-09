using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FirstAgendaItemAction : AppAction<EmptyRequest, EmptyActionResult>
{
    private readonly FirstAgendaItemCounter counter;

    public FirstAgendaItemAction(FirstAgendaItemCounter counter)
    {
        this.counter = counter;
    }

    public Task<EmptyActionResult> Execute(EmptyRequest model)
    {
        Console.WriteLine($"First Agenda Item: {DateTime.Now:HH:mm:ss}");
        counter.Increment();
        return Task.FromResult(new EmptyActionResult());
    }
}