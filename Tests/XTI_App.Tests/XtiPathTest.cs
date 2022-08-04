using NUnit.Framework;
using XTI_App.Abstractions;

namespace XTI_App.Tests;

internal sealed class XtiPathTest
{
    [Test]
    public void ShouldParsePathWithApp()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current");
        Assert.That(xtiPath.App, Is.EqualTo("Fake"));
        Assert.That(xtiPath.Version, Is.EqualTo("Current"));
        Assert.That(xtiPath.Group, Is.EqualTo(""));
        Assert.That(xtiPath.Action, Is.EqualTo(""));
        Assert.That(xtiPath.Modifier, Is.EqualTo(""));
    }

    [Test]
    public void ShouldParsePathWithGroup()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current/Group1");
        Assert.That(xtiPath.App, Is.EqualTo("Fake"));
        Assert.That(xtiPath.Version, Is.EqualTo("Current"));
        Assert.That(xtiPath.Group, Is.EqualTo("Group1"));
        Assert.That(xtiPath.Action, Is.EqualTo(""));
        Assert.That(xtiPath.Modifier, Is.EqualTo(""));
    }

    [Test]
    public void ShouldParsePathWithAction()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current/Group1/Action1");
        Assert.That(xtiPath.App, Is.EqualTo("Fake"));
        Assert.That(xtiPath.Version, Is.EqualTo("Current"));
        Assert.That(xtiPath.Group, Is.EqualTo("Group1"));
        Assert.That(xtiPath.Action, Is.EqualTo("Action1"));
        Assert.That(xtiPath.Modifier, Is.EqualTo(""));
    }

    [Test]
    public void ShouldParsePathWithModifier()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current/Group1/Action1/Modifier");
        Assert.That(xtiPath.Modifier, Is.EqualTo("Modifier"), "Should parse path with modifier");
    }

    [Test]
    public void ShouldParseODataQuery()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current/odata/SomeQuery/$query");
        Assert.That(xtiPath.Group, Is.EqualTo("SomeQuery"));
        Assert.That(xtiPath.Action, Is.EqualTo("Get"));
        Assert.That(xtiPath.Modifier, Is.EqualTo(""));
    }

    [Test]
    public void ShouldParseODataQueryWithModifier()
    {
        var xtiPath = XtiPath.Parse("/Fake/Current/odata/SomeQuery/Mod1/$query");
        Assert.That(xtiPath.Group, Is.EqualTo("SomeQuery"));
        Assert.That(xtiPath.Action, Is.EqualTo("Get"));
        Assert.That(xtiPath.Modifier, Is.EqualTo(new ModifierKey("Mod1")));
    }
}