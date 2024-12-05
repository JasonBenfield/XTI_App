using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppApiActionBuilder
{
    public string ActionName { get; }

    XtiPath ActionPath();

    IAppApiAction Build();
}
