﻿using Microsoft.Extensions.DependencyInjection;
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
        var xtiPathAccessor = scope.ServiceProvider.GetRequiredService<ActionRunnerXtiPathAccessor>();
        xtiPathAccessor.FinishPath(groupName, actionName);
        var xtiPath = xtiPathAccessor.Value();
        result = await verifyActionIsRequired(environment, factory, xtiPath);
        if (result == Results.None)
        {
            result = await run(environment, factory, xtiPath, stoppingToken);
        }
        return result;
    }

    private async Task<Results> verifyActionIsRequired(XtiEnvironment environment, IActionRunnerFactory factory, XtiPath xtiPath)
    {
        var result = Results.None;
        var session = factory.CreateTempLogSession();
        try
        {
            var action = getApiAction(factory);
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
            await session.LogException(AppEventSeverity.Values.CriticalError, ex, $"Unexpected error in {path}");
            await session.EndRequest();
            result = Results.Error;
        }
        return result;
    }

    private async Task<Results> run(XtiEnvironment environment, IActionRunnerFactory factory, XtiPath xtiPath, CancellationToken stoppingToken)
    {
        var result = Results.None;
        var session = factory.CreateTempLogSession();
        var path = xtiPath.Format();
        try
        {
            await session.StartRequest(path);
            var action = getApiAction(factory);
            await action.Execute(new EmptyRequest(), stoppingToken);
            result = Results.Succeeded;
        }
        catch (Exception ex)
        {
            if (!environment.IsProduction())
            {
                Console.WriteLine($"Unexpected error in {path}\r\n{ex}");
            }
            result = Results.Error;
            await session.LogException
            (
                AppEventSeverity.Values.CriticalError,
                ex,
                $"Unexpected error in {path}"
            );
        }
        finally
        {
            await session.EndRequest();
        }
        return result;
    }

    private AppApiAction<EmptyRequest, EmptyActionResult> getApiAction(IActionRunnerFactory factory)
    {
        var api = factory.CreateAppApi();
        return api
            .Group(groupName)
            .Action<EmptyRequest, EmptyActionResult>(actionName);
    }
}