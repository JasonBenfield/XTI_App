namespace XTI_App.Fakes;

public sealed record SampleResultData(string ResultCode, int ResultValue)
{
    public SampleResultData()
        : this("", 0)
    {
    }
}
