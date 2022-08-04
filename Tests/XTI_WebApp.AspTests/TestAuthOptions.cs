using XTI_App.Abstractions;

namespace XTI_WebApp.AspTests;

public sealed class TestAuthOptions
{
    public bool IsEnabled { get; set; }
    public string SessionKey { get; set; } = "";
    public UserContextModel? User { get; set; }
}
