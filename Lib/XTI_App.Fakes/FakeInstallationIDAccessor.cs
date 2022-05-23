using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeInstallationIDAccessor : InstallationIDAccessor
{
    private int installationID;

    public FakeInstallationIDAccessor() : this(123) { }

    public FakeInstallationIDAccessor(int installationID)
    {
        this.installationID = installationID;
    }

    public Task<int> Value() => Task.FromResult(installationID);

    public void SetInstallationID(int installationID) => this.installationID = installationID;
}
