using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using XTI_App.Abstractions;
using XTI_App.Secrets;
using XTI_TempLog;

namespace XTI_WebApp.Scheduled;

public sealed class ScheduledAppEnvironmentContext : IAppEnvironmentContext
{
    private readonly ISystemUserCredentials systemUserCredentials;
    private readonly InstallationIDAccessor installationIDAccessor;

    public ScheduledAppEnvironmentContext(ISystemUserCredentials systemUserCredentials, InstallationIDAccessor installationIDAccessor)
    {
        this.systemUserCredentials = systemUserCredentials;
        this.installationIDAccessor = installationIDAccessor;
    }

    public async Task<AppEnvironment> Value()
    {
        var credentials = await systemUserCredentials.Value();
        var firstMacAddress = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString())
            .FirstOrDefault()
            ?? "";
        var installationID = await installationIDAccessor.Value();
        var env = new AppEnvironment
        (
            credentials.UserName,
            firstMacAddress,
            Environment.MachineName,
            $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
            installationID
        );
        return env;
    }
}