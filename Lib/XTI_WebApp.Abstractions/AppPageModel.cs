namespace XTI_WebApp.Abstractions;

public sealed class AppPageModel
{
    public string[] PreStyleSheets { get; set; } = new string[0];
    public string[] PostStyleSheets { get; set; } = new string[0];
    public string[] PreScripts { get; set; } = new string[0];
    public string[] PostScripts { get; set; } = new string[0];
}
