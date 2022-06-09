namespace XTI_WebAppClient;

public interface IXtiToken
{
    Task<string> Value();
}