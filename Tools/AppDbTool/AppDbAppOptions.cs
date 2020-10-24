namespace AppDbApp
{
    public sealed class AppDbAppOptions
    {
        public string Command { get; set; }
        public string BackupFilePath { get; set; }
        public bool Force { get; set; }
    }
}
