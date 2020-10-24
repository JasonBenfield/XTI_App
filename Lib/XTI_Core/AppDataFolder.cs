using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XTI_Core
{
    public sealed class AppDataFolder
    {
        private readonly List<string> subFolderNames = new List<string>();

        public AppDataFolder()
            : this(new string[] { })
        {
        }

        private AppDataFolder(IEnumerable<string> subFolderNames)
        {
            this.subFolderNames.AddRange(subFolderNames);
        }

        public string Path()
            => System.IO.Path.Combine
            (
                new[]
                {
                    Environment.GetEnvironmentVariable("XTI_AppData")
                }
                .Union(subFolderNames)
                .ToArray()
            );

        public string FilePath(string fileName) => System.IO.Path.Combine(Path(), fileName);

        public AppDataFolder WithSubFolder(string name)
            => new AppDataFolder(subFolderNames.Union(new[] { name }));

        public AppDataFolder WithHostEnvironment(IHostEnvironment hostEnv)
            => WithSubFolder(hostEnv.EnvironmentName);

        public void TryCreate()
        {
            var path = Path();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
