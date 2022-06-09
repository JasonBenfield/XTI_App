using NUnit.Framework;
using XTI_WebAppClient;

namespace XTI_WebApp.Tests;

internal sealed class ObjectToQueryStringTest
{
    [Test]
    public void ShouldOutputEmptyString_WhenObjectIsNull()
    {
        var objToQuery = new ObjectToQueryString(null);
        Assert.That(objToQuery.Value, Is.EqualTo(""));
    }

    [Test]
    public void ShouldOutputSingleParameter()
    {
        var objToQuery = new ObjectToQueryString
        (
            new
            {
                ID = 1
            }
        );
        Assert.That(objToQuery.Value, Is.EqualTo("?ID=1"));
    }

    [Test]
    public void ShouldOutputMultipleParameters()
    {
        var objToQuery = new ObjectToQueryString
        (
            new
            {
                ID = 1,
                Name = "xartogg"
            }
        );
        Assert.That(objToQuery.Value, Is.EqualTo("?ID=1&Name=xartogg"));
    }

    [Test]
    public void ShouldNotOutputNullParameters()
    {
        var objToQuery = new ObjectToQueryString
        (
            new
            {
                ID = 1,
                Name = (string?)null
            }
        );
        Assert.That(objToQuery.Value, Is.EqualTo("?ID=1"));
    }

    [Test]
    public void ShouldOutputNestedObject()
    {
        var objToQuery = new ObjectToQueryString
        (
            new
            {
                ID = (int?)1,
                Department = new
                {
                    DepartmentNumber = 16,
                    Description = "IT"
                }
            }
        );
        Assert.That(objToQuery.Value, Is.EqualTo("?ID=1&Department.DepartmentNumber=16&Department.Description=IT"));
    }
}
