namespace XTI_App.Abstractions;

public static class ResultContainer
{
    public static ResultContainer<T> Create<T>(T data) => new ResultContainer<T>(data);
}

public sealed class ResultContainer<T>
{
    public ResultContainer()
    {
    }

    public ResultContainer(T data)
    {
        Data = data;
    }

    public T? Data { get; set; }
}