using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiActionBuilder<TRequest, TResult> : IAppApiActionBuilder
{
    private readonly IServiceProvider sp;
    private readonly XtiPath groupPath;
    private readonly IAppApiUser user;
    private readonly ActionThrottledLogPathBuilder<AppApiActionBuilder<TRequest, TResult>> throttledLogPathBuilder;
    private ActionScheduleBuilder<AppApiActionBuilder<TRequest, TResult>> schedule;
    private string friendlyName = "";
    private readonly ResourceAccessBuilder accessBuilder;
    private Func<AppActionValidation<TRequest>> createValidation;
    private Func<AppAction<TRequest, TResult>> createExecution;
    private AppApiAction<TRequest, TResult>? builtAction;
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
        createExecution = () => new EmptyAppAction<TRequest, TResult>(() => default!);
    }

    private static readonly Func<AppActionValidation<TRequest>> defaultValidation =
        () => new AppActionValidationNotRequired<TRequest>();

    public string ActionName { get; private set; } = "";

    public AppApiActionBuilder<TRequest, TResult> Named(string actionName)
    {
        ActionName = actionName;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithFriendlyName(string friendlyName)
    {
        this.friendlyName = friendlyName;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> AllowAnonymousAccess()
    {
        accessBuilder.AllowAnonymous();
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> ResetAccess()
    {
        accessBuilder.Reset();
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> ResetAccess(ResourceAccess access)
    {
        accessBuilder.Reset(access);
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> ResetAccessWithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.Reset(roleNames);
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithAllowed(roleNames);
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithoutAllowed(params AppRoleName[] roleNames)
    {
        accessBuilder.WithoutAllowed(roleNames);
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithValidation<TValidation>()
        where TValidation : AppActionValidation<TRequest> =>
        WithValidation(() => sp.GetRequiredService<TValidation>());

    public AppApiActionBuilder<TRequest, TResult> WithValidation(Func<AppActionValidation<TRequest>> createValidation)
    {
        this.createValidation = createValidation;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithExecution<TExecution>()
        where TExecution : AppAction<TRequest, TResult>
    {
        WithExecution(() => sp.GetRequiredService<TExecution>());
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> WithExecution(Func<AppAction<TRequest, TResult>> createExecution)
    {
        this.createExecution = createExecution;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> AlwaysLogRequestData()
    {
        requestDataLoggingType = RequestDataLoggingTypes.Always;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> LogRequestDataOnError()
    {
        requestDataLoggingType = RequestDataLoggingTypes.OnError;
        return this;
    }

    public AppApiActionBuilder<TRequest, TResult> LogResultData()
    {
        isResultDataLoggingEnabled = true;
        return this;
    }

    public ActionThrottledLogIntervalBuilder<AppApiActionBuilder<TRequest, TResult>> ThrottleRequestLogging() =>
        throttledLogPathBuilder.RequestIntervalBuilder;

    public ActionThrottledLogIntervalBuilder<AppApiActionBuilder<TRequest, TResult>> ThrottleExceptionLogging() =>
        throttledLogPathBuilder.ExceptionIntervalBuilder;

    public ActionScheduleBuilder<AppApiActionBuilder<TRequest, TResult>> RunContinuously()
    {
        schedule = new(this, ScheduledActionTypes.Continuous);
        return schedule;
    }

    public ActionScheduleBuilder<AppApiActionBuilder<TRequest, TResult>> RunUntilSuccess()
    {
        schedule = new(this, ScheduledActionTypes.PeriodicUntilSuccess);
        return schedule;
    }

    public AppApiAction<TRequest, TResult> Build()
    {
        builtAction ??= BuildAction();
        return builtAction;
    }

    public XtiPath ActionPath() => groupPath.WithAction(ActionName);

    private AppApiAction<TRequest, TResult> BuildAction()
    {
        var actionPath = ActionPath();
        var schedule = this.schedule.Build();
        return new AppApiAction<TRequest, TResult>
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
