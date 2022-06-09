using System.Text.RegularExpressions;

namespace XTI_WebAppClient;

public class ClientNumericValues<T> 
    where T : ClientNumericValue
{
    private static readonly Regex whitespaceRegex = new Regex("\\s+");

    private readonly List<T> values = new List<T>();

    protected ClientNumericValues()
    {
    }

    protected T Add(T value)
    {
        values.Add(value);
        return value;
    }

    public T Value(int value) =>
        values.FirstOrDefault(nv => nv.Equals(value)) ?? GetDefault();

    public T Value(string displayText)
    {
        return values
            .FirstOrDefault
            (
                v => whitespaceRegex.Replace(v.DisplayText, "")
                    .Equals
                    (
                        whitespaceRegex.Replace(displayText, ""),
                        StringComparison.OrdinalIgnoreCase
                    )
            )
            ?? GetDefault();
    }

    public T GetDefault() => values.First();

    public T[] GetAll() => values.ToArray();
}