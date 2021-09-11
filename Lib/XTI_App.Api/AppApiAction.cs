using System;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using XTI_Core;
using XTI_Forms;

namespace XTI_App.Api
{
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
            string friendlyName
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
        }

        public XtiPath Path { get; }
        public string ActionName { get => Path.Action.DisplayText.Replace(" ", ""); }
        public string FriendlyName { get; }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => user.HasAccess(Access);

        public Task<bool> IsOptional()
        {
            var action = createAction();
            if (action is OptionalAction<TModel, TResult> optional)
            {
                return optional.IsOptional();
            }
            return Task.FromResult(false);
        }

        public async Task<TResult> Invoke(TModel model)
        {
            var result = await Execute(model);
            return result.Data;
        }

        public async Task<ResultContainer<TResult>> Execute(TModel model)
        {
            await EnsureUserHasAccess();
            var errors = new ErrorList();
            if (model is Form form)
            {
                form.Validate(errors);
                ensureValidInput(errors);
            }
            var validation = createValidation();
            await validation.Validate(errors, model);
            ensureValidInput(errors);
            var action = createAction();
            var actionResult = await action.Execute(model);
            return new ResultContainer<TResult>(actionResult);
        }

        private async Task EnsureUserHasAccess()
        {
            var hasAccess = await HasAccess();
            if (!hasAccess)
            {
                throw new AccessDeniedException(Path);
            }
        }

        private static void ensureValidInput(ErrorList errors)
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
}
