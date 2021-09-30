using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_Core;

namespace XTI_App.Hosting
{
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

        public async Task<Results> Run()
        {
            Results result;
            using (var scope = sp.CreateScope())
            {
                var xtiPathAccessor = scope.ServiceProvider.GetService<ActionRunnerXtiPathAccessor>();
                xtiPathAccessor.FinishPath(groupName, actionName);
                var xtiPath = xtiPathAccessor.Value();
                var factory = scope.ServiceProvider.GetService<IActionRunnerFactory>();
                result = await verifyActionIsRequired(factory, xtiPath);
                if (result == Results.None)
                {
                    result = await run(factory, xtiPath);
                }
            }
            return result;
        }

        private async Task<Results> verifyActionIsRequired(IActionRunnerFactory factory, XtiPath xtiPath)
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
                await session.StartRequest(path);
                await session.LogException(AppEventSeverity.Values.CriticalError, ex, $"Unexpected error in {path}");
                await session.EndRequest();
                result = Results.Error;
            }
            return result;
        }

        private async Task<Results> run(IActionRunnerFactory factory, XtiPath xtiPath)
        {
            Results result = Results.None;
            var session = factory.CreateTempLogSession();
            var path = xtiPath.Format();
            try
            {
                await session.StartRequest(path);
                var action = getApiAction(factory);
                await action.Execute(new EmptyRequest());
                result = Results.Succeeded;
            }
            catch (Exception ex)
            {
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
}
