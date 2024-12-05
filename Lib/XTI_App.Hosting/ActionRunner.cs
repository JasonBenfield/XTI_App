using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;

namespace XTI_App.Hosting;

public sealed class ActionRunner
{
    private readonly IServiceProvider sp;
    private readonly string groupName;
    private readonly string actionName;

    public ActionRunner
    (
        IServiceProvider sp,
        string groupName,
        string actionName
    )
    {
        this.sp = sp;
        this.groupName = groupName;
        this.actionName = actionName;
    }

    public enum Results
    {
        None,
        Succeeded,
        Error,
        NotRequired
    }

    public async Task<Results> Run(CancellationToken stoppingToken)
    {
        Results result;
        using var scope = sp.CreateScope();
        var environment = scope.ServiceProvider.GetRequiredService<XtiEnvironment>();
        var factory = scope.ServiceProvider.GetRequiredService<IActionRunnerFactory>();
        var xtiBasePath = scope.ServiceProvider.GetRequiredService<XtiBasePath>();
        var xtiPath = xtiBasePath.Finish(groupName, actionName);
        result = await VerifyActionIsRequired(environment, factory, xtiPath);
        if (result == Results.None)
        {
            result = await Run(environment, factory, xtiPath, stoppingToken);
        }
        return result;
    }

    private async Task<Results> VerifyActionIsRequired(XtiEnvironment environment, IActionRunnerFactory factory, XtiPath xtiPath)
    {
        var result = Results.None;
        var session = factory.CreateTempLogSession();
        try
        {
            var action = GetApiAction(factory);
            var isOptional = await action.IsOptional();
            if (isOptional)
            {
                result = Results.NotRequired;
            }
        }
        catch (Exception ex)
        {
            var path = xtiPath.Format();
            if (!environment.IsProduction())
            {
                Console.WriteLine($"Unexpected error in {path}\r\n{ex}");
            }
            await session.StartRequest(path);
            var parentEventKey = "";
            if(ex is AppClientException clientEx)
            {
                parentEventKey = clientEx.LogEntryKey;
            }
            await session.LogException(AppEventSeverity.Values.CriticalError, ex, $"Unexpected error in {path}", parentEventKey);
            await session.EndRequest();
            result = Results.Error;
        }
        return result;
    }

    private async Task<Results> Run(XtiEnvironment xtiEnv, IActionRunnerFactory factory, XtiPath xtiPath, CancellationToken stoppingToken)
    {
        var result = Results.None;
        var tempLogSession = factory.CreateTempLogSession();
        var path = xtiPath.Format();
        try
        {
            await tempLogSession.StartRequest(path);
            var action = GetApiAction(factory);
            await action.Execute(new EmptyRequest(), stoppingToken);
            result = Results.Succeeded;
        }
        catch (Exception ex)
        {
            if (!xtiEnv.IsProduction())
            {
                Console.WriteLine($"Unexpected error in {path}\r\n{ex}");
            }
            result = Results.Error;
            var parentEventKey = "";
            if (ex is AppClientException clientEx)
            {
                parentEventKey = clientEx.LogEntryKey;
            }
            await tempLogSession.LogException
            (
                AppEventSeverity.Values.CriticalError,
                ex,
                $"Unexpected error in {path}",
                parentEventKey
            );
        }
        finally
        {
            await tempLogSession.EndRequest();
        }
        return result;
    }

    private AppApiAction<EmptyRequest, EmptyActionResult> GetApiAction(IActionRunnerFactory factory)
    {
        var api = factory.CreateAppApi();
        return api
            .Group(groupName)
            .Action<EmptyRequest, EmptyActionResult>(actionName);
    }
}