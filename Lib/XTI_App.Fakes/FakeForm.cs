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

        Password = AddTextInput(nameof(Password));
        Password.MaxLength = 100;
        Password.SetValue("");
        Password.MustNotBeNull();
        Password.Protect();

        Confirm = AddTextInput(nameof(Confirm));
        Confirm.MaxLength = 100;
        Confirm.SetValue("");
        Confirm.MustNotBeNull();
        Confirm.MustBeEqualToField(Password);

        NegativeNumber = AddInt32Input(nameof(NegativeNumber));
        NegativeNumber.SetValue(-99);
        NegativeNumber.MustNotBeNull();
        NegativeNumber.MustBeNegative();
        PositiveNumber = AddInt32Input(nameof(PositiveNumber));
        PositiveNumber.SetValue(23);
        PositiveNumber.MustNotBeNull();
        PositiveNumber.MustBePositive();
        RangedNumber = AddInt32Input(nameof(RangedNumber));
        RangedNumber.SetValue(15);
        RangedNumber.MustNotBeNull();
        RangedNumber.MustBeInRange
        (
            Int32RangeConstraint.FromOnOrAbove(10).ToBelow(20)
        );
        DateOnlyValue = AddDateInput(nameof(DateOnlyValue));
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
    public InputField<string> Password { get; }
    public InputField<string> Confirm { get; }
    public InputField<int?> NegativeNumber { get; }
    public InputField<int?> PositiveNumber { get; }
    public InputField<int?> RangedNumber { get; }
    public InputField<DateOnly?> DateOnlyValue { get; }
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