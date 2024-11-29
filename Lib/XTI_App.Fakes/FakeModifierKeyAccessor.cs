using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class FakeModifierKeyAccessor : IModifierKeyAccessor
{
    private ModifierKey modifierKey = ModifierKey.Default;

    public void SetValue(ModifierKey modifierKey)
    {
        this.modifierKey  = modifierKey;
    }

    public ModifierKey Value() => modifierKey;
}
