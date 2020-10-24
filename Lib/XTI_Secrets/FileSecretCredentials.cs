using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using XTI_Core;

namespace XTI_Secrets
{
    public sealed class FileSecretCredentials : SecretCredentials
    {
        private readonly AppDataFolder appDataFolder;

        public FileSecretCredentials(IHostEnvironment hostEnv, string key, IDataProtector dataProtector)
            : base(key, dataProtector)
        {
            appDataFolder = new AppDataFolder()
                .WithHostEnvironment(hostEnv)
                .WithSubFolder("Secrets");
        }

        protected override async Task<string> Load(string key)
        {
            string text;
            var filePath = getFilePath(key);
            if (File.Exists(getFilePath(key)))
            {
                using (var reader = new StreamReader(filePath))
                {
                    text = await reader.ReadToEndAsync();
                }
            }
            else
            {
                text = "";
            }
            return text;
        }

        protected override async Task Persist(string key, string encryptedText)
        {
            appDataFolder.TryCreate();
            using (var writer = new StreamWriter(getFilePath(key), false))
            {
                await writer.WriteAsync(encryptedText);
            }
        }

        private string getFilePath(string key) => appDataFolder.FilePath($"{key}.secret");
    }
}
