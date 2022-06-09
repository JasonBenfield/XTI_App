using NUnit.Framework;
using System.Text.Json;
using XTI_App.Fakes;
using XTI_WebAppClient;

namespace XTI_WebApp.Tests;

internal sealed class NumericValueClientTest
{
    [Test]
    public void ShouldDeserializeNumericValue()
    {
        var serialized = JsonSerializer.Serialize(EmployeeType.Values.Permanent);
        var deserialized = JsonSerializer.Deserialize<EmployeeTypeClient>(serialized);
        Assert.That(deserialized, Is.EqualTo(EmployeeTypeClient.Values.Permanent), "Should deserialize numeric value client");
    }

    public sealed class EmployeeTypeClient : ClientNumericValue
    {
        public sealed class EmployeeTypesClient : ClientNumericValues<EmployeeTypeClient>
        {
            internal EmployeeTypesClient()
            {
                None = Add(new EmployeeTypeClient(0, "None"));
                Temp = Add(new EmployeeTypeClient(10, "Temp"));
                Permanent = Add(new EmployeeTypeClient(15, "Permanent"));
            }

            public EmployeeTypeClient None
            {
                get;
            }

            public EmployeeTypeClient Temp
            {
                get;
            }

            public EmployeeTypeClient Permanent
            {
                get;
            }
        }

        public static readonly EmployeeTypesClient Values = new EmployeeTypesClient();

        public EmployeeTypeClient(int value, string displayText) : base(value, displayText)
        {
        }
    }
}