using System.Text.RegularExpressions;

namespace XTI_WebAppClient;

public partial class ClientNumericValues<T>
    where T : ClientNumericValue
{
    private readonly List<T> values = new();

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
                v => WhitespaceRegex().Replace(v.DisplayText, "")
                    .Equals
                    (
                        WhitespaceRegex().Replace(displayText, ""),
                        StringComparison.OrdinalIgnoreCase
                    )
            )
            ?? GetDefault();
    }

    public T GetDefault() => values.First();

    public T[] GetAll() => values.ToArray();

    [GeneratedRegex("\\s+")]
    private static partial Regex WhitespaceRegex();
}