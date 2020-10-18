namespace XTI_App.DB
{
    public sealed class AppDbName : XtiDbName
    {
        public AppDbName(string environmentName) : base(environmentName, "App")
        {
        }
    }
}
