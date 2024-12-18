using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;
using XTI_TempLog;

namespace XTI_App.Hosting;

public sealed class ActionRunner
{
    private readonly IServiceProvider sp;
    private readonly XtiEnvironment xtiEnv;
    private readonly XtiBasePath xtiBasePath;
    private readonly string groupName;
    private readonly string actionName;

    public ActionRunner
    (
        IServiceProvider sp,
        XtiEnvironment xtiEnv,
        XtiBasePath xtiBasePath,
        string groupName,
        string actionName
    )
    {
        this.sp = sp;
        this.xtiEnv = xtiEnv;
        this.xtiBasePath = xtiBasePath;
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
        try
        {
            var factory = scope.ServiceProvider.GetRequiredService<IActionRunnerFactory>();
            var session = factory.CreateTempLogSession();
            var api = factory.CreateAppApi();
            var action = GetApiAction(api);
            var xtiPath = xtiBasePath.Finish(groupName, actionName);
            result = await VerifyActionIsRequired(xtiPath, session, action);
            if (result == Results.None)
            {
                result = await Run(xtiPath, session, action, stoppingToken);
            }
        }
        catch
        {
            result = Results.Error;
        }
        return result;
    }

    private async Task<Results> VerifyActionIsRequired(XtiPath xtiPath, TempLogSession tempLogSession, AppApiAction<EmptyRequest, EmptyActionResult> action)
    {
        var result = Results.None;
        try
        {
            var isOptional = await action.IsOptional();
            if (isOptional)
            {
                result = Results.NotRequired;
            }
        }
        catch (Exception ex)
        {
            var path = xtiPath.Format();
            if (!xtiEnv.IsProduction())
            {
                Console.WriteLine($"Unexpected error in {path}\r\n{ex}");
            }
            await tempLogSession.StartRequest(path);
            var parentEventKey = "";
            if (ex is AppClientException clientEx)
            {
                parentEventKey = clientEx.LogEntryKey;
            }
            await tempLogSession.LogException(AppEventSeverity.Values.CriticalError, ex, $"Unexpected error in {path}", parentEventKey);
            await tempLogSession.EndRequest();
            result = Results.Error;
        }
        return result;
    }

    private async Task<Results> Run(XtiPath xtiPath, TempLogSession tempLogSession, AppApiAction<EmptyRequest, EmptyActionResult> action, CancellationToken stoppingToken)
    {
        var result = Results.None;
        var path = xtiPath.Format();
        try
        {
            await tempLogSession.StartRequest(path);
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

    private AppApiAction<EmptyRequest, EmptyActionResult> GetApiAction(IAppApi api) =>
        api
            .Group(groupName)
            .Action<EmptyRequest, EmptyActionResult>(actionName);
}