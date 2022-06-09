namespace XTI_WebAppClient;

public sealed class AnonymousXtiToken : IXtiToken
{
    public Task<string> Value() => Task.FromResult("");
}
