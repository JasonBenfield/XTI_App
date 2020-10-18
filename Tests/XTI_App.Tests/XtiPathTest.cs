using NUnit.Framework;

namespace XTI_App.Tests
{
    public sealed class XtiPathTest
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
    }
}
