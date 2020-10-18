namespace XTI_Secrets
{
    public sealed class SecretOptions
    {
        public static readonly string Secret = "Secret";

        public string ApplicationName { get; set; }
        public string KeyDirectoryPath { get; set; }
    }
}
