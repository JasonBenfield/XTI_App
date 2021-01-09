using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using XTI_Core;
using XTI_Forms;

namespace XTI_App.Tests
{
    public sealed class FormTest
    {
        [Test]
        public void ShouldAddTextInputToForm()
        {
            var form = new TestForm();
            var fields = form.ToModel().Fields;
            var nameField = fields.FirstOrDefault(t => t.Name.Equals("TestText"));
            Assert.That(nameField, Is.Not.Null, "Should add text input");
            Assert.That(nameField, Is.TypeOf(typeof(InputFieldModel)), "Should add text input");
            Assert.That(nameField.Caption, Is.EqualTo("Test Text"), "Should add text input");
            var inputField = (InputFieldModel)nameField;
            Assert.That(inputField.MaxLength, Is.EqualTo(100), "Should add text input");
        }

        [Test]
        public void ShouldFailNotNullValidation_WhenValueIsNull()
        {
            var form = new TestForm();
            form.TestText.MustNotBeNull();
            form.TestText.SetValue(null);
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should add error");
            Assert.That(errors[0].Message, Is.EqualTo(FormErrors.MustNotBeNull));
        }

        [Test]
        public void ShouldPassValidation()
        {
            var form = new TestForm();
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(0), "Should pass validation");
        }

        [Test]
        public void ShouldNotBeValid_WhenValueIsLessThanTheLowerBound()
        {
            var form = new TestForm();
            form.PositiveNumber.SetValue(-42);
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should not be valid when value is less than the lower bound");
            Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.LowerRangeExclusive, 0)));
        }

        [Test]
        public void ShouldNotBeValid_WhenValueIsGreaterThanTheUpperBound()
        {
            var form = new TestForm();
            form.NegativeNumber.SetValue(42);
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should not be valid when value is greater than the upper bound");
            Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.UpperRangeExclusive, "0")));
        }

        [Test]
        [TestCase(5), TestCase(20), TestCase(25)]
        public void ShouldNotBeValid_WhenValueIsOutsideTheRange(int? value)
        {
            var form = new TestForm();
            form.RangedNumber.SetValue(value);
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should not be valid when value is outside the bounds");
            if (value < 10)
            {
                Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.LowerRangeInclusive, "10")));
            }
            else if (value >= 20)
            {
                Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.UpperRangeExclusive, "20")));
            }
        }

        [Test]
        public void ShouldNotBeValid_WhenValueIsEqualToTheExcludedUpperBound()
        {
            var form = new TestForm();
            form.NegativeNumber.SetValue(0);
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should not be valid when value is greater than the upper bound");
            Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.UpperRangeExclusive, "0")));
        }

        [Test]
        public void ShouldNotBeValid_WhenTextIsLongerThanTheMaxLength()
        {
            var form = new TestForm();
            form.TestText.MaxLength = 10;
            form.TestText.SetValue("".PadLeft(11, 'A'));
            var errorList = new ErrorList();
            form.Validate(errorList);
            var errors = errorList.Errors().ToArray();
            Assert.That(errors.Length, Is.EqualTo(1), "Should not be valid when value is greater than the upper bound");
            Assert.That(errors[0].Message, Is.EqualTo(string.Format(FormErrors.MustNotExceedLength, 10)));
        }

        [Test]
        public void ShouldImportValues()
        {
            var form = new TestForm();
            form.Import(new Dictionary<string, object>
            {
                { "TestForm_TestText", "Test Import" },
                { "TestForm_NegativeNumber", -100 },
                { "TestForm_PositiveNumber", 100 },
                { "TestForm_RangedNumber", 11 },
                { "TestForm_DecimalDropDown", 5.2 },
                { "TestForm_Question", true },
                { "TestForm_TestComplex_Field1", "Something" },
                { "TestForm_TestComplex_Field2", 55 }
            });
            Assert.That(form.TestText.Value(), Is.EqualTo("Test Import"));
            Assert.That(form.NegativeNumber.Value(), Is.EqualTo(-100));
            Assert.That(form.PositiveNumber.Value(), Is.EqualTo(100));
            Assert.That(form.RangedNumber.Value(), Is.EqualTo(11));
            Assert.That(form.DecimalDropDown.Value(), Is.EqualTo(5.2M));
            Assert.That(form.Question.Value(), Is.True);
            Assert.That(form.TestComplex.Field1.Value(), Is.EqualTo("Something"));
            Assert.That(form.TestComplex.Field2.Value(), Is.EqualTo(55));
        }

        [Test]
        public void ShouldExportValues()
        {
            var form = new TestForm();
            var imported = new Dictionary<string, object>
            {
                { "TestForm_TestText", "Test Import" },
                { "TestForm_NegativeNumber", -100 },
                { "TestForm_PositiveNumber", 100 },
                { "TestForm_RangedNumber", 11 },
                { "TestForm_DecimalDropDown", 5.2 },
                { "TestForm_Question", true },
                { "TestForm_TestComplex_Field1", "Something" },
                { "TestForm_TestComplex_Field2", 55 }
            };
            form.Import(imported);
            var exported = form.Export();
            Assert.That(exported, Is.EquivalentTo(imported));
        }

        private sealed class TestForm : Form
        {
            public TestForm() : base("TestForm")
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
    }
}