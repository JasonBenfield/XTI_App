namespace XTI_WebApp.Abstractions;

public interface IAppClientDomain
{
    Task<string> Value(string appName, string version);
}