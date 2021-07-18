using XTI_App.Abstractions;

namespace XTI_App.Api
{
    public interface ICachedUserContext : IUserContext
    {
        void ClearCache(int userID);
        void ClearCache(AppUserName userName);
    }
}
