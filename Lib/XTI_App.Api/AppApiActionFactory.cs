using System;

namespace XTI_App.Api
{
    public class AppApiActionFactory
    {
        private readonly XtiPath path;
        private readonly ResourceAccess defaultAccess;
        private readonly IAppApiUser user;

        public AppApiActionFactory(AppApiGroup group)
        {
            path = group.Path;
            defaultAccess = group.Access;
            user = group.User;
        }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>
        (
            string actionName,
            Func<AppActionValidation<TModel>> createValidation,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Action(actionName, defaultAccess, createValidation, createExecution, friendlyName);
        }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>
        (
            string actionName,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Action(actionName, defaultAccess, defaultCreateValidation<TModel>(), createExecution, friendlyName);
        }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>
        (
            string actionName,
            ResourceAccess access,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Action
            (
                actionName,
                access,
                defaultCreateValidation<TModel>(),
                createExecution,
                friendlyName
            );
        }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>
        (
            string actionName,
            ResourceAccess access,
            Func<AppActionValidation<TModel>> createValidation,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = null
        )
        {
            if (string.IsNullOrWhiteSpace(actionName)) { throw new ArgumentException($"{nameof(actionName)} is required"); }
            return new AppApiAction<TModel, TResult>
            (
                path.WithAction(actionName),
                access,
                user,
                createValidation ?? defaultCreateValidation<TModel>(),
                createExecution ?? defaultCreateAction<TModel, TResult>(),
                friendlyName
            );
        }

        public static Func<AppActionValidation<TModel>> defaultCreateValidation<TModel>() =>
            () => new AppActionValidationNotRequired<TModel>();

        public static Func<AppAction<TModel, TResult>> defaultCreateAction<TModel, TResult>() =>
            () => new EmptyAppAction<TModel, TResult>();

    }
}
