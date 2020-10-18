namespace XTI_App.EF
{
    public sealed class AppDbName : XtiDbName
    {
        public AppDbName(string environmentName) : base(environmentName, "App")
        {
        }
    }
}
