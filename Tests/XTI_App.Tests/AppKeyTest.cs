﻿using NUnit.Framework;
using System.ComponentModel;
using System.Text.Json;
using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Tests;

internal sealed class AppKeyTest
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
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized, jsonOptions);
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
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new AppKeyJsonConverter());
        jsonOptions.Converters.Add(new AppVersionKeyJsonConverter());
        var serialized = JsonSerializer.Serialize(appModel, jsonOptions);
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized, jsonOptions);
        Assert.That(deserialized.AppKey, Is.Null);
    }

    [Test]
    public void ShouldDeserializeAppKeyAsString()
    {
        var appKey = new AppKey("Test", AppType.Values.WebApp);
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new AppKeyJsonConverter());
        jsonOptions.Converters.Add(new AppVersionKeyJsonConverter());
        var serialized = "{ \"AppKey\": \"Test|Web App\" }";
        var deserialized = XtiSerializer.Deserialize<AppModel>(serialized, jsonOptions);
        Assert.That(deserialized.AppKey, Is.EqualTo(appKey));
    }

    [Test]
    public void ShouldConvertFromStringToAppKey()
    {
        var appKey = new AppKey("Test", AppType.Values.WebApp);
        var serialized = appKey.Serialize();
        var typeConverter = TypeDescriptor.GetConverter(typeof(AppKey));
        var converted = typeConverter.ConvertFrom(serialized);
        Assert.That(converted, Is.EqualTo(appKey));
    }

    private sealed class AppModel
    {
        public AppKey? AppKey { get; set; } = AppKey.Unknown;
        public AppVersionKey VersionKey { get; set; } = AppVersionKey.None;
        public string Title { get; set; } = "";
    }
}