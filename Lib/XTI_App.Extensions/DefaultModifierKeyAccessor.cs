using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Extensions;

public sealed class DefaultModifierKeyAccessor : IModifierKeyAccessor
{
    private readonly ModifierKey modKey = ModifierKey.Default;

    public ModifierKey Value() => modKey;
}
