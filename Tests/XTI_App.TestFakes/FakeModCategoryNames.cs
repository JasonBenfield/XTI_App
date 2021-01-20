namespace XTI_App.TestFakes
{
    public sealed class FakeModCategoryNames
    {
        public static readonly FakeModCategoryNames Instance = new FakeModCategoryNames();
        public ModifierCategoryName Department { get; } = new ModifierCategoryName(nameof(Department));
    }
}
