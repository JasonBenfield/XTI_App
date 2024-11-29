namespace XTI_WebApp.Api;

public static class LinkModelExtensions
{
    public static bool IsXtiPath(this LinkModel link) =>
        link.Url.StartsWith("~/");

    public static XtiPath GetXtiPath(this LinkModel link, XtiBasePath basePath)
    {
        var path = basePath.Value;
        if (link.IsXtiPath())
        {
            var url = link.Url;
            var queryIndex = url.IndexOf('?');
            if (queryIndex > -1)
            {
                url = url.Substring(0, queryIndex);
            }
            var parts = url.Split('/');
            path = path.WithNewGroup(string.IsNullOrWhiteSpace(parts[1]) ? "Home" : parts[1])
                .WithAction(parts.Length > 2 ? parts[2] : "Index")
                .WithModifier(new ModifierKey(parts.Length > 3 ? parts[3] : ""));
        }
        return path;
    }
}
