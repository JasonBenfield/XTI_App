using Microsoft.Extensions.Hosting;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Extensions;

internal sealed class FileInstallationIDAccessor : InstallationIDAccessor
{
    private readonly IHostEnvironment hostEnv;

    public FileInstallationIDAccessor(IHostEnvironment hostEnv)
    {
        this.hostEnv = hostEnv;
    }

    public async Task<int> Value()
    {
        var installationID = 0;
        var path = Path.Combine(hostEnv.ContentRootPath, "installation.json");
        if (File.Exists(path))
        {
            var content = await File.ReadAllTextAsync(path);
            var info = XtiSerializer.Deserialize(content, () => new InstallationInfo(0));
            installationID = info.InstallationID;
        }
        return installationID;
    }

    private sealed record InstallationInfo(int InstallationID);
}
