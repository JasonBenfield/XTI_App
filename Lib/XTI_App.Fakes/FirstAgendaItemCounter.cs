namespace XTI_App.Fakes;

public sealed class FirstAgendaItemCounter
{
    public int Value { get; private set; }

    public void Increment() => Value++;
}