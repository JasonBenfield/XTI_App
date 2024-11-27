using XTI_App.Abstractions;
using XTI_Core;
using XTI_Forms;
using XTI_TempLog;

namespace XTI_App.Api;

public sealed class AppApiAction<TModel, TResult> : IAppApiAction
{
    private readonly IAppApiUser user;
    private readonly Func<AppActionValidation<TModel>> createValidation;
    private readonly Func<AppAction<TModel, TResult>> createAction;

    public AppApiAction
    (
        XtiPath path,
        ResourceAccess access,
        IAppApiUser user,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, TResult>> createAction,
        string friendlyName,
        ThrottledLogPath throttledLogPath
    )
    {
        path.EnsureActionResource();
        Access = access;
        Path = path;
        FriendlyName = string.IsNullOrWhiteSpace(friendlyName)
            ? string.Join(" ", new CamelCasedWord(path.Action.DisplayText).Words())
            : friendlyName;
        this.user = user;
        this.createValidation = createValidation;
        this.createAction = createAction;
        ThrottledLogPath = throttledLogPath;
    }

    public XtiPath Path { get; }
    public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }
    public ThrottledLogPath ThrottledLogPath { get; }

    public Task<bool> IsOptional()
    {
        var action = createAction();
        if (action is OptionalAction<TModel, TResult> optional)
        {
            return optional.IsOptional();
        }
        return Task.FromResult(false);
    }

    public async Task<TResult> Invoke(TModel model, CancellationToken stoppingToken = default)
    {
        var result = await Execute(model, stoppingToken);
        return result.Data!;
    }

    public async Task<ResultContainer<TResult>> Execute(TModel model, CancellationToken stoppingToken = default)
    {
        await user.EnsureUserHasAccess(Path);
        var errors = new ErrorList();
        if (model is Form form)
        {
            form.Validate(errors);
            EnsureValidInput(errors);
        }
        var validation = createValidation();
        await validation.Validate(errors, model, stoppingToken);
        EnsureValidInput(errors);
        var action = createAction();
        var actionResult = await action.Execute(model, stoppingToken);
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
        var modelTemplate = new ValueTemplateFromType(typeof(TModel)).Template();
        var resultTemplate = new ValueTemplateFromType(typeof(TResult)).Template();
        return new AppApiActionTemplate(Path.Action.DisplayText, FriendlyName, Access, modelTemplate, resultTemplate);
    }

    public override string ToString() => $"{GetType().Name} {FriendlyName}";
}