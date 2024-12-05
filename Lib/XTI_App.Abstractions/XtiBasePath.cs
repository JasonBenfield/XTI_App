namespace XTI_App.Abstractions;

public sealed class XtiBasePath
{
    public XtiBasePath(AppKey appKey, AppVersionKey versionKey)
    {
        VersionKey = versionKey;
        Value = new XtiPath(appKey)
            .WithVersion(VersionKey);
    }

    public AppVersionKey VersionKey { get; }

    public XtiPath Value { get; }

    public XtiPath Finish(ResourceGroupName groupName, ResourceName actionName) =>
        Finish(groupName.DisplayText, actionName.DisplayText);

    public XtiPath Finish(string groupName, string actionName) =>
        Value
            .WithGroup(groupName.Replace(" ", ""))
            .WithAction(actionName.Replace(" ", ""));
}
