using System;
using System.Collections.Generic;
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
        public string ActionName { get => Path.Action.DisplayText; }
        public string FriendlyName { get; }
        public ResourceAccess Access { get; }

        public Task<bool> HasAccess() => user.HasAccess(Access);

        public async Task<object> Execute(object model) => await Execute((TModel)model);

        public async Task<ResultContainer<TResult>> Execute(TModel model)
        {
            await EnsureUserHasAccess();
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

        private async Task EnsureUserHasAccess()
        {
            var hasAccess = await HasAccess();
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
