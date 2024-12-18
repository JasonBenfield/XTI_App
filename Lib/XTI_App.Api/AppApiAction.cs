using XTI_App.Abstractions;
using XTI_Core;
using XTI_Forms;
using XTI_TempLog;

namespace XTI_App.Api;

public sealed class AppApiAction<TRequest, TResult> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<AppActionValidation<TRequest>> createValidation;
    private readonly Func<AppAction<TRequest, TResult>> createAction;
    private readonly ThrottledLogXtiPath throttledLogPath;

    public AppApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<AppActionValidation<TRequest>> createValidation,
        Func<AppAction<TRequest, TResult>> createAction,
        string friendlyName,
        RequestDataLoggingTypes requestDataLoggingType,
        bool isResultDataLoggingEnabled,
        ThrottledLogXtiPath throttledLogPath,
        ScheduledAppAgendaItemOptions schedule
    )
    {
        path.EnsureActionResource();
        Access = access;
        Path = path;
        FriendlyName = string.IsNullOrWhiteSpace(friendlyName)
            ? string.Join(" ", new CamelCasedWord(path.Action.DisplayText).Words())
            : friendlyName;
        RequestDataLoggingType = requestDataLoggingType;
        IsResultDataLoggingEnabled = isResultDataLoggingEnabled;
        Schedule = schedule;
        this.user = user;
        this.createValidation = createValidation;
        this.createAction = createAction;
        this.throttledLogPath = throttledLogPath;
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }
    public ScheduledAppAgendaItemOptions Schedule { get; }
    public RequestDataLoggingTypes RequestDataLoggingType { get; }
    public bool IsResultDataLoggingEnabled { get; }
    public ThrottledLogPath ThrottledLogPath(XtiBasePath xtiBasePath) => throttledLogPath.Value(xtiBasePath);

    public Task<bool> IsOptional()
    {
        var action = createAction();
        if (action is OptionalAction<TRequest, TResult> optional)
        {
            return optional.IsOptional();
        }
        return Task.FromResult(false);
    }

    public async Task<TResult> Invoke(TRequest requestData, CancellationToken stoppingToken = default)
    {
        var result = await Execute(requestData, stoppingToken);
        return result.Data!;
    }

    public async Task<ResultContainer<TResult>> Execute(TRequest requestData, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Path);
        var errors = new ErrorList();
        if (requestData is Form form)
        {
            form.Validate(errors);
            EnsureValidInput(errors);
        }
        var validation = createValidation();
        await validation.Validate(errors, requestData, stoppingToken);
        EnsureValidInput(errors);
        var action = createAction();
        var actionResult = await action.Execute(requestData, stoppingToken);
        return new ResultContainer<TResult>(actionResult);
    }

    private static void EnsureValidInput(ErrorList errors)
    {
        if (errors.Any())
        {
            throw new ValidationFailedException(errors.Errors());
        }
    }

    public AppApiActionTemplate Template()
    {
        var requestTemplate = new ValueTemplateFromType(typeof(TRequest)).Template();
        var resultTemplate = new ValueTemplateFromType(typeof(TResult)).Template();
        return new AppApiActionTemplate(Path.Action.DisplayText, FriendlyName, Access, requestTemplate, resultTemplate);
    }

    public override string ToString() => $"{GetType().Name} {FriendlyName}";
}