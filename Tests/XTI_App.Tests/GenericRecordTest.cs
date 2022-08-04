using NUnit.Framework;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_WebAppClient;

namespace XTI_App.Tests;

internal sealed class GenericRecordTest
{
    [Test]
    public void ShouldDeserializeGenericRecord()
    {
        var record = XtiSerializer.Deserialize<GenericRecord>
        (
            @"
{
    ""ID"": 1,
    ""DisplayText"": ""Whatever"",
    ""TimeAdded"": ""2022-06-12 19:10:00""
}
"
        );
        Assert.That(record.ValueOrDefault<int>("ID"), Is.EqualTo(1));
        Assert.That(record.ValueOrDefault("DisplayText", ""), Is.EqualTo("Whatever"));
        Assert.That(record.ValueOrDefault<DateTimeOffset>("TimeAdded"), Is.EqualTo(new DateTimeOffset(new DateTime(2022, 6, 12, 19, 10, 0))));
    }

    [Test]
    public void ShouldDeserializeArrayOfGenericRecords()
    {
        var records = XtiSerializer.Deserialize
        (
            @"
[
    {
        ""ID"": 1,
        ""DisplayText"": ""Whatever 1"",
        ""TimeAdded"": ""2022-06-12 19:10:00""
    },
    {
        ""ID"": 2,
        ""DisplayText"": ""Whatever 2"",
        ""TimeAdded"": ""2022-06-12 19:10:00""
    }
]
", () => new GenericRecord[0]
        );
        Assert.That
        (
            records.Select(r => r.ValueOrDefault("DisplayText", "")),
            Is.EqualTo(new[] { "Whatever 1", "Whatever 2" })
        );
    }

    [Test]
    public void ShouldDeserializeNestedGenericRecord()
    {
        var record = XtiSerializer.Deserialize<GenericRecord>
        (
            @"
{
    ""ID"": 1,
    ""DisplayText"": ""Whatever"",
    ""TimeAdded"": ""2022-06-12 19:10:00"",
    ""Item"": { ""ItemID"": 100, ""Name"": ""Item 100"" }
}
"
        );
        Assert.That(record.ValueOrDefault<int>("Item.ItemID"), Is.EqualTo(100));
        Assert.That(record.ValueOrDefault("Item.Name", ""), Is.EqualTo("Item 100"));
    }

    [Test]
    public void ShouldSerializeGenericRecord()
    {
        var record = new GenericRecord
        (
            new Dictionary<string, object>
            {
                { "ID", 1 },
                { "DisplayText", "Whatever" },
                { "TimeAdded", new DateTimeOffset(new DateTime(2022, 6, 12, 19, 10, 0)) },
                { 
                    "Item", 
                    new GenericRecord
                    (
                        new Dictionary<string,object> { { "ItemID", 100 }, { "Name", "Item 100" } }
                    )
                }
            }
        );
        var serialized = XtiSerializer.Serialize(record);
        Console.WriteLine(serialized);
        var deserialized = XtiSerializer.Deserialize<GenericRecord>(serialized);
        Assert.That(deserialized.ValueOrDefault<int>("ID"), Is.EqualTo(1));
        Assert.That(deserialized.ValueOrDefault("DisplayText", ""), Is.EqualTo("Whatever"));
        Assert.That(deserialized.ValueOrDefault<DateTimeOffset>("TimeAdded"), Is.EqualTo(new DateTimeOffset(new DateTime(2022, 6, 12, 19, 10, 0))));
        Assert.That(deserialized.ValueOrDefault<int>("Item.ItemID"), Is.EqualTo(100));
        Assert.That(deserialized.ValueOrDefault("Item.Name", ""), Is.EqualTo("Item 100"));
    }
}
