using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiActionBuilder<TModel, TResult> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private readonly ActionThrottledLogPathBuilder<AppApiActionBuilder<TModel, TResult>> throttledLogPathBuilder;
    private ActionScheduleBuilder<AppApiActionBuilder<TModel, TResult>> schedule;
    private string friendlyName = "";
    private readonly ResourceAccessBuilder accessBuilder;
    private Func<AppActionValidation<TModel>> createValidation;
    private Func<AppAction<TModel, TResult>> createExecution;
    private AppApiAction<TModel, TResult>? builtAction;
    private RequestDataLoggingTypes requestDataLoggingType = RequestDataLoggingTypes.Never;
    private bool isResultDataLoggingEnabled;

    internal AppApiActionBuilder
    (
        IServiceProvider sp,
        XtiPath groupPath,
        IAppApiUser user,
        ResourceAccessBuilder groupAccessBuilder
    )
    {
        this.sp = sp;
        this.groupPath = groupPath;
        this.user = user;
        accessBuilder = new ResourceAccessBuilder(groupAccessBuilder);
        schedule = new(this);
        throttledLogPathBuilder = new(this);
        createValidation = defaultValidation;
        createExecution = () => new EmptyAppAction<TModel, TResult>(() => default!);
    }

    private static readonly Func<AppActionValidation<TModel>> defaultValidation =
        () => new AppActionValidationNotRequired<TModel>();

    public string ActionName { get; private set; } = "";

    public AppApiActionBuilder<TModel, TResult> Named(string actionName)
    {
        ActionName = actionName;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithValidation<TValidation>()
        where TValidation : AppActionValidation<TModel> =>
        WithValidation(() => sp.GetRequiredService<TValidation>());

    public AppApiActionBuilder<TModel, TResult> WithValidation(Func<AppActionValidation<TModel>> createValidation)
    {
        this.createValidation = createValidation;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithExecution<TExecution>()
        where TExecution : AppAction<TModel, TResult>
    {
        WithExecution(() => sp.GetRequiredService<TExecution>());
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> WithExecution(Func<AppAction<TModel, TResult>> createExecution)
    {
        this.createExecution = createExecution;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> AlwaysLogRequestData()
    {
        requestDataLoggingType = RequestDataLoggingTypes.Always;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> LogRequestDataOnError()
    {
        requestDataLoggingType = RequestDataLoggingTypes.OnError;
        return this;
    }

    public AppApiActionBuilder<TModel, TResult> LogResultData()
    {
        isResultDataLoggingEnabled = true;
        return this;
    }

    public ActionThrottledLogIntervalBuilder<AppApiActionBuilder<TModel, TResult>> ThrottleRequestLogging() =>
        throttledLogPathBuilder.RequestIntervalBuilder;

    public ActionThrottledLogIntervalBuilder<AppApiActionBuilder<TModel, TResult>> ThrottleExceptionLogging() =>
        throttledLogPathBuilder.ExceptionIntervalBuilder;

    public ActionScheduleBuilder<AppApiActionBuilder<TModel, TResult>> RunContinuously()
    {
        schedule = new(this, ScheduledActionTypes.Continuous);
        return schedule;
    }

    public ActionScheduleBuilder<AppApiActionBuilder<TModel, TResult>> RunUntilSuccess()
    {
        schedule = new(this, ScheduledActionTypes.PeriodicUntilSuccess);
        return schedule;
    }

    public AppApiAction<TModel, TResult> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    public XtiPath ActionPath() => groupPath.WithAction(ActionName);

    private AppApiAction<TModel, TResult> BuildAction()
    {
        var actionPath = ActionPath();
        var schedule = this.schedule.Build();
        return new AppApiAction<TModel, TResult>
        (
            actionPath,
            accessBuilder.Build(),
            user,
            createValidation,
            createExecution,
            new AppActionFriendlyName(friendlyName, ActionName).Value,
            requestDataLoggingType,
            isResultDataLoggingEnabled,
            throttledLogPathBuilder.Build(actionPath),
            schedule
        );
    }

    IAppApiAction IAppApiActionBuilder.Build() => Build();
}
