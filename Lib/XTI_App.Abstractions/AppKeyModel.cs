namespace XTI_App.Abstractions
{
    public sealed class AppKeyModel
    {
        public AppKeyModel() { }

        public AppKeyModel(AppKey appKey)
        {
            AppName = appKey.Name.Value;
            AppType = appKey.Type.Value;
        }

        public string AppName { get; set; }
        public int AppType { get; set; }

        public AppKey ToAppKey()
            => new AppKey
            (
                new AppName(AppName),
                XTI_App.Abstractions.AppType.Values.Value(AppType)
            );
    }
}
