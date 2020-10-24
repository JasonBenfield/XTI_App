namespace XTI_App.Api
{
    public sealed class AppEnvironment
    {
        public string UserName { get; set; } = "";
        public string RequesterKey { get; set; } = "";
        public string RemoteAddress { get; set; } = "";
        public string UserAgent { get; set; } = "";
        public XtiPath Path { get; set; } = new XtiPath("", "");
    }
}
