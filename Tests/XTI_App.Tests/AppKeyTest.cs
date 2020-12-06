using NUnit.Framework;

namespace XTI_App.Tests
{
    public sealed class AppKeyTest
    {
        [Test]
        public void ShouldParseAppKey()
        {
            var appKey = new AppKey("Test", AppType.Values.WebApp);
            var serialized = appKey.Serialize();
            var parsed = AppKey.Parse(serialized);
            Assert.That(parsed.Name, Is.EqualTo(new AppName("Test")), "Should parse app key");
            Assert.That(parsed.Type, Is.EqualTo(AppType.Values.WebApp), "Should parse app key");
        }
    }
}
