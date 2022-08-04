namespace XTI_WebApp.Abstractions;

public interface IBasicAuthValidator
{
    Task<bool> IsValid(string username, string password);
}
