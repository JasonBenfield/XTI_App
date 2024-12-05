namespace XTI_App.Fakes;

public sealed class FakeError
{
    public bool ThrowError { get; set; }
    public Exception Error { get; set; } = new Exception("Testing");

    public void ThrowIfRequired()
    {
        if (ThrowError)
        {
            throw Error;
        }
    }
}
