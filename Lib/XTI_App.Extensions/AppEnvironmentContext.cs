using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using XTI_App.Abstractions;
using XTI_TempLog;

namespace XTI_App.Extensions;

public sealed class AppEnvironmentContext : IAppEnvironmentContext
{
    private readonly ICurrentUserName currentUserName;
    private readonly AppKey appKey;

    public AppEnvironmentContext(ICurrentUserName currentUserName, AppKey appKey)
    {
        this.currentUserName = currentUserName;
        this.appKey = appKey;
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
        return new AppEnvironment
        (
            userName.Value,
            firstMacAddress,
            Environment.MachineName,
            $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
            appKey.Type.DisplayText
        );
    }
}