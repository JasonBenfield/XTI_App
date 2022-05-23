using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Extensions;

public sealed class AppEnvironmentContext : IAppEnvironmentContext
{
    private readonly ICurrentUserName currentUserName;
    private readonly InstallationIDAccessor installationIDAccessor;

    public AppEnvironmentContext(ICurrentUserName currentUserName, InstallationIDAccessor installationIDAccessor)
    {
        this.currentUserName = currentUserName;
        this.installationIDAccessor = installationIDAccessor;
    }

    public async Task<AppEnvironment> Value()
    {
        var userName = await currentUserName.Value();
        var firstMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault()
            ?? "";
        var installationID = await installationIDAccessor.Value();
        return new AppEnvironment
        (
            userName.Value,
            firstMacAddress,
            Environment.MachineName,
            $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
            installationID
        );
    }
}