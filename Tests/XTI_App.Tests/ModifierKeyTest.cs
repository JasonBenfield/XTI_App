namespace XTI_App.Tests;

internal sealed class ModifierKeyTest
{
    [Test]
    public void ShouldRemoveSpacesFromModKey()
    {
        var modKey = new ModifierKey("One Two");
        Assert.That(modKey.Value, Is.EqualTo("onetwo"));
        Assert.That(modKey.DisplayText, Is.EqualTo("OneTwo"));
    }
}
