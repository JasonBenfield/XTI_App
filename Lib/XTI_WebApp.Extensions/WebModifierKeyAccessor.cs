using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_WebApp.Extensions;

public sealed class WebModifierKeyAccessor : IModifierKeyAccessor
{
    private readonly WebXtiPathAccessor xtiPathAccessor;

    public WebModifierKeyAccessor(WebXtiPathAccessor xtiPathAccessor)
    {
        this.xtiPathAccessor = xtiPathAccessor;
    }

    public ModifierKey Value()
    {
        var xtiPath = xtiPathAccessor.Value();
        return xtiPath.Modifier;
    }
}
