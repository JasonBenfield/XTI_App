using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using XTI_App;
using XTI_Tool;
using XTI_VersionToolApi;

namespace XTI_Version
{
    public sealed class VersionOutput
    {
        public async Task Output(AppVersion version, string outputPath)
        {
            var app = await version.App();
            var appKey = app.Key();
            var output = new VersionToolOutput
            {
                VersionKey = version.Key().DisplayText,
                VersionType = version.Type().DisplayText,
                VersionNumber = version.Version().ToString(3),
                DevVersionNumber = version.NextPatch().ToString(3),
                AppName = appKey.Name.DisplayText,
                AppType = appKey.Type.DisplayText
            };
            new XtiProcessData().Output(output);
            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                var dirPath = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using var writer = new StreamWriter(outputPath, false);
                writer.WriteLine
                (
                    JsonSerializer.Serialize
                    (
                        new VersionRecord
                        {
                            Key = version.Key().Value,
                            Type = version.Type().DisplayText,
                            Version = version.Version().ToString()
                        }
                    )
                );
            }
        }

        private sealed class VersionRecord
        {
            public string Key { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
        }

    }
}
