using NUnit.Framework;
using System.Text.Json;
using XTI_App.Abstractions;

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

        [Test]
        public void ShouldDeserializeAppKey()
        {
            var appKey = new AppKey("Test", AppType.Values.WebApp);
            var appModel = new AppModel
            {
                AppKey = appKey,
                VersionKey = new AppVersionKey(12),
                Title = "Testing"
            };
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new AppKeyJsonConverter());
            jsonOptions.Converters.Add(new AppVersionKeyJsonConverter());
            var serialized = JsonSerializer.Serialize(appModel, jsonOptions);
            var deserialized = JsonSerializer.Deserialize<AppModel>(serialized, jsonOptions);
            Assert.That(deserialized.AppKey, Is.EqualTo(appKey));
            Assert.That(deserialized.VersionKey, Is.EqualTo(new AppVersionKey(12)));
            Assert.That(deserialized.Title, Is.EqualTo(appModel.Title));
        }

        private sealed class AppModel
        {
            public AppKey AppKey { get; set; }
            public AppVersionKey VersionKey { get; set; }
            public string Title { get; set; }
        }
    }
}
