using Microsoft.Extensions.Hosting;
using XTI_App.Api;

namespace XTI_ConsoleApp;

public sealed class StopApplicationAction : AppAction<EmptyRequest, EmptyActionResult>
{
    private readonly IHostApplicationLifetime lifetime;

    public StopApplicationAction(IHostApplicationLifetime lifetime)
    {
        this.lifetime = lifetime;
    }

    public Task<EmptyActionResult> Execute(EmptyRequest model)
    {
        lifetime.StopApplication();
        return Task.FromResult(new EmptyActionResult());
    }
}