using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XTI_Secrets
{
    public sealed class FileSecretCredentials : SecretCredentials
    {
        private readonly string directoryPath;

        public FileSecretCredentials(string environmentName, string key, IDataProtector dataProtector)
            : base(key, dataProtector)
        {
            directoryPath = Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "XTI",
                "Secrets",
                environmentName
            );
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
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (var writer = new StreamWriter(getFilePath(key), false))
            {
                await writer.WriteAsync(encryptedText);
            }
        }

        private string getFilePath(string key) => Path.Combine(directoryPath, $"{key}.secret");
    }
}
