using System;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class AppApiAction<TModel, TResult> : IAppApiAction
    {
        public AppApiAction
        (
            XtiPath path,
            ResourceAccess access,
            IAppApiUser user,
            Func<AppActionValidation<TModel>> createValidation,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName
        )
        {
            path.EnsureActionResource();
            Access = access;
            Path = path;
            FriendlyName = string.IsNullOrWhiteSpace(friendlyName)
                ? new FriendlyNameFromActionName(path.Action.DisplayText).Value
                : friendlyName;
            this.user = user;
            this.createValidation = createValidation;
            this.createExecution = createExecution;
        }

        private readonly IAppApiUser user;
        private readonly Func<AppActionValidation<TModel>> createValidation;
        private readonly Func<AppAction<TModel, TResult>> createExecution;

        public XtiPath Path { get; }
        public string FriendlyName { get; }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess(ModifierKey modKey) =>
            user.HasAccess(Path, Access, modKey);

        public async Task<object> Execute(ModifierKey modifier, object model) =>
            await Execute(modifier, (TModel)model);

        public Task<ResultContainer<TResult>> Execute(TModel model) =>
            Execute(ModifierKey.Default, model);

        public async Task<ResultContainer<TResult>> Execute
        (
            ModifierKey modifier, TModel model
        )
        {
            await EnsureUserHasAccess(modifier);
            var errors = new ErrorList();
            var validation = createValidation();
            await validation.Validate(errors, model);
            if (errors.Any())
            {
                throw new ValidationFailedException(errors.Errors());
            }
            var action = createExecution();
            var actionResult = await action.Execute(model);
            return new ResultContainer<TResult>(actionResult);
        }

        private async Task EnsureUserHasAccess(ModifierKey modifier)
        {
            var hasAccess = await HasAccess(modifier);
            if (!hasAccess)
            {
                throw new AccessDeniedException(Path);
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
