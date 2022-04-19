using XTI_App.Abstractions;

namespace DemoConsoleApp;

public static class DemoInfo
{
    public static readonly AppKey AppKey = new AppKey(new AppName("Demo"), AppType.Values.ConsoleApp);
}