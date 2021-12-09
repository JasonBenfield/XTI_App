namespace XTI_App.Api;

public interface IAppApiUser
{
    Task<bool> HasAccess(ResourceAccess resourceAccess);
}