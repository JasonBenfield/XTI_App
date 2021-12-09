namespace XTI_Forms;

internal sealed class FormattedValue<T>
{
    private readonly T value;

    public FormattedValue(T value)
    {
        this.value = value;
    }

    public string Format()
    {
        string str;
        if (value == null)
        {
            str = "null";
        }
        else if (value is DateTimeOffset dateTimeValue)
        {
            if (dateTimeValue.Date == dateTimeValue)
            {
                str = dateTimeValue.ToString("M/dd/yyyy");
            }
            else
            {
                str = dateTimeValue.ToString("M/dd/yyyy h:mm:ss TT");
            }
        }
        else
        {
            str = value.ToString() ?? "";
        }
        return str;
    }
}