namespace XTI_App.Tests;

internal sealed class InstallerUserNameTest
{
    [Test]
    public void ShouldParseUserName()
    {
        var installerUserName = InstallerUserName.Parse(new InstallerUserName(Environment.MachineName).UserName);
        Assert.That(installerUserName.MachineName.ToLower(), Is.EqualTo(Environment.MachineName.ToLower()));
    }

    [Test]
    public void ShouldParseOldVersionUserName()
    {
        var installerUserName = InstallerUserName.Parse(new AppUserName("xti_inst[machinename]"));
        Assert.That(installerUserName.MachineName.ToLower(), Is.EqualTo("machinename"));
    }

    [Test]
    public void ShouldStartWith()
    {
        var installerUserName = new InstallerUserName(Environment.MachineName);
        Assert.That(installerUserName.UserName.Value.StartsWith("xti_inst2"), Is.True);
    }
}
