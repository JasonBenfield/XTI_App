namespace XTI_App.DB
{
    public sealed class DbOptions
    {
        public static readonly string DB = "DB";

        public string Source { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
