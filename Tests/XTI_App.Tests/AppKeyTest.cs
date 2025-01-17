﻿using System.ComponentModel;
using XTI_Core;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class AppKeyTest
{
    [Test]
    public void ShouldParseAppKey()
    {
        var appKey = AppKey.WebApp("Test");
        var appKey2 = AppKey.WebApp("CpwAuthenticator");
        var appName = new AppName("Cpw Authenticator");
        var serialized = appKey.Serialize();
        var parsed = AppKey.Parse(serialized);
        Assert.That(parsed.Name, Is.EqualTo(new AppName("Test")), "Should parse app key");
        Assert.That(parsed.Type, Is.EqualTo(AppType.Values.WebApp), "Should parse app key");
    }

    [Test]
    public void ShouldDeserializeAppKey()
    {
        var appKey = AppKey.WebApp("Test");
        var appModel = new AppModel
        {
            AppKey = appKey,
            VersionKey = new AppVersionKey(12),
            Title = "Testing"
        };
        var serialized = XtiSerializer.Serialize(appModel);
        Console.WriteLine(serialized);
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized);
        deserialized.WriteToConsole();
        Assert.That(deserialized.AppKey, Is.EqualTo(appKey));
        Assert.That(deserialized.VersionKey, Is.EqualTo(new AppVersionKey(12)));
        Assert.That(deserialized.Title, Is.EqualTo(appModel.Title));
    }

    [Test]
    public void ShouldDeserializeAppKeyAsNull_WhenNull()
    {
        var appModel = new AppModel
        {
            AppKey = null,
            VersionKey = new AppVersionKey(12),
            Title = "Testing"
        };
        var serialized = XtiSerializer.Serialize(appModel);
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized);
        Assert.That(deserialized.AppKey, Is.Null);
    }

    [Test]
    public void ShouldDeserializeAppKeyAsString()
    {
        var appKey = AppKey.WebApp("Test");
        var serialized = "{ \"AppKey\": \"Test|Web App\" }";
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized);
        Assert.That(deserialized.AppKey, Is.EqualTo(appKey));
    }

    [Test]
    public void ShouldConvertFromStringToAppKey()
    {
        var appKey = AppKey.WebApp("Test");
        var serialized = appKey.Serialize();
        var typeConverter = TypeDescriptor.GetConverter(typeof(AppKey));
        var converted = typeConverter.ConvertFrom(serialized);
        Assert.That(converted, Is.EqualTo(appKey));
    }

    [Test]
    public void AppNamesShouldBeEqual()
    {
        Assert.That(new AppName("ScheduledJobs"), Is.EqualTo(new AppName("Scheduled Jobs")));
    }

    private sealed class AppModel
    {
        public AppKey? AppKey { get; set; } = AppKey.Unknown;
        public AppVersionKey VersionKey { get; set; } = AppVersionKey.None;
        public string Title { get; set; } = "";
    }
}