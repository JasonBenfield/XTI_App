using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class ResourceNameTest
{
    [Test]
    public void ShouldRemoveSpacesFromResourceName()
    {
        var resourceName = new ResourceName("MoveToPermanentV1");
        resourceName.WriteToConsole();
    }
}
