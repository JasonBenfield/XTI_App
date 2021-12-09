namespace XTI_App.Api;

public sealed class ResultContainer<T>
{
    public ResultContainer(T data)
    {
        Data = data;
    }

    public T Data { get; }
}