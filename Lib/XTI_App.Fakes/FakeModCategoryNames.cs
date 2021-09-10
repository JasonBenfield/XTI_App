using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeModCategoryNames
    {
        public static readonly FakeModCategoryNames Instance = new FakeModCategoryNames();
        public ModifierCategoryName Department { get; } = new ModifierCategoryName(nameof(Department));
    }
}
