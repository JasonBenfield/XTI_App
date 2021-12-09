using XTI_Core;

namespace XTI_App.Fakes;

public sealed class EmployeeType : NumericValue
{
    public sealed class EmployeeTypes : NumericValues<EmployeeType>
    {
        internal EmployeeTypes() : base(new EmployeeType(0, "None"))
        {
            None = DefaultValue;
            Temp = Add(new EmployeeType(10, "Temp"));
            Permanent = Add(new EmployeeType(15, "Permanent"));
        }
        public EmployeeType None { get; }
        public EmployeeType Temp { get; }
        public EmployeeType Permanent { get; }
    }

    public static readonly EmployeeTypes Values = new EmployeeTypes();

    private EmployeeType(int value, string displayText) : base(value, displayText)
    {
    }
}