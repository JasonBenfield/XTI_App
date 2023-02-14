using XTI_App.Fakes;

namespace XTI_App.Tests;

internal sealed class SystemUserNameTest
{
    [Test]
    public void ShouldParseUserName()
    {
        var systemUserName = SystemUserName.Parse(new SystemUserName(FakeInfo.AppKey, Environment.MachineName).UserName);
        Assert.That(systemUserName.AppKey, Is.EqualTo(FakeInfo.AppKey));
        Assert.That(systemUserName.MachineName.ToLower(), Is.EqualTo(Environment.MachineName.ToLower()));
    }

    [Test]
    public void ShouldParseOldVersionUserName()
    {
        var systemUserName = SystemUserName.Parse(new AppUserName("xti_sys[fake:service app][machinename]"));
        Assert.That(systemUserName.AppKey, Is.EqualTo(FakeInfo.AppKey));
        Assert.That(systemUserName.MachineName.ToLower(), Is.EqualTo("machinename"));
    }

    [Test]
    public void ShouldStartWith()
    {
        var systemUserName = new SystemUserName(FakeInfo.AppKey, Environment.MachineName);
        StringAssert.StartsWith("xti_sys2", systemUserName.UserName.Value);
    }
}
