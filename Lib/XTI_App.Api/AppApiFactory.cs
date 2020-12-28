namespace XTI_App.Api
{
    public abstract class AppApiFactory
    {
        public AppApi Create(IAppApiUser user) => _Create(user);
        public AppApi CreateForSuperUser() => _Create(new AppApiSuperUser());
        public AppApiTemplate CreateTemplate() => CreateForSuperUser().Template();

        protected abstract AppApi _Create(IAppApiUser user);
    }
}
