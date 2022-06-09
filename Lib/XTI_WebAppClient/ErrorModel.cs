using System.Text;

namespace XTI_WebAppClient;

public sealed class ErrorModel
{
    public ErrorModel()
        : this("", "", "")
    {
    }

    public ErrorModel(string message, string caption, string source)
    {
        Message = message;
        Caption = caption;
        Source = source;
    }

    public string Message { get; set; }
    public string Caption { get; set; }
    public string Source { get; set; }

    public string Format()
    {
        var formatted = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(Source))
        {
            formatted.Append(Source);
        }
        if (!string.IsNullOrWhiteSpace(Caption))
        {
            if (formatted.Length > 0)
            {
                formatted.Append(": ");
            }
            formatted.Append(Caption);
        }
        if (!string.IsNullOrWhiteSpace(Message))
        {
            if (formatted.Length > 0)
            {
                formatted.Append(": ");
            }
            formatted.Append(Message);
        }
        return formatted.ToString();
    }

    public override string ToString() => $"{nameof(ErrorModel)} {Format()}";
}