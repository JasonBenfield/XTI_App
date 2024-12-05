using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Api;

public sealed class AppActionFriendlyName
{
    public AppActionFriendlyName(string friendlyName, string actionName)
    {
        Value = string.IsNullOrWhiteSpace(friendlyName)
            ? string.Join(" ", new CamelCasedWord(actionName).Words())
            : friendlyName;
    }

    public string Value { get; }
}
