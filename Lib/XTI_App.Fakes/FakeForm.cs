using XTI_Forms;

namespace XTI_App.Fakes;

public sealed class FakeForm : Form
{
    public FakeForm() : base("TestForm")
    {
        TestText = AddTextInput(nameof(TestText));
        TestText.MaxLength = 100;
        TestText.SetValue("Initial Value");
        TestText.MustNotBeNull();
        NegativeNumber = AddInt32Input(nameof(NegativeNumber));
        NegativeNumber.SetValue(-99);
        NegativeNumber.MustNotBeNull();
        NegativeNumber.AddConstraints(Int32RangeConstraint.Negative());
        PositiveNumber = AddInt32Input(nameof(PositiveNumber));
        PositiveNumber.SetValue(23);
        PositiveNumber.MustNotBeNull();
        PositiveNumber.AddConstraints(Int32RangeConstraint.Positive());
        RangedNumber = AddInt32Input(nameof(RangedNumber));
        RangedNumber.SetValue(15);
        RangedNumber.MustNotBeNull();
        RangedNumber.AddConstraints
        (
            Int32RangeConstraint.FromOnOrAbove(10).ToBelow(20)
        );
        DecimalDropDown = AddDecimalDropDown
        (
            nameof(DecimalDropDown),
            new DropDownItem<decimal?>(5.1M, "Item 1"),
            new DropDownItem<decimal?>(5.2M, "Item 2"),
            new DropDownItem<decimal?>(5.3M, "Item 3")
        );
        DecimalDropDown.ItemCaption = "Select...";
        Question = AddBooleanDropDown(nameof(Question));
        TestComplex = AddComplex(nameof(TestComplex), (prefix, name) => new FakeComplexField(prefix, name));
    }

    public InputField<string> TestText { get; }
    public InputField<int?> NegativeNumber { get; }
    public InputField<int?> PositiveNumber { get; }
    public InputField<int?> RangedNumber { get; }
    public DropDownField<decimal?> DecimalDropDown { get; }
    public DropDownField<bool?> Question { get; }
    public FakeComplexField TestComplex { get; }
}

public sealed class FakeComplexField : ComplexField
{
    public FakeComplexField(string prefix, string name)
        : base(prefix, name)
    {
        Field1 = AddTextInput(nameof(Field1));
        Field2 = AddInt32Input(nameof(Field2));
    }
    public InputField<string> Field1 { get; }
    public InputField<int?> Field2 { get; }
}