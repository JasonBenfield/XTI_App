namespace XTI_App.Fakes;

public sealed class FakeEntityID
{
    private int currentID = new Random((int)DateTime.UtcNow.Ticks).Next(100000);

    public int Next()
    {
        currentID++;
        return currentID;
    }
}