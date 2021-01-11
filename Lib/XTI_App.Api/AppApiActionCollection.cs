using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
    public sealed class AppApiActionCollection : IAppApiActionCollection
    {
        private readonly XtiPath path;
        private readonly ResourceAccess access;
        private readonly IAppApiUser user;
        private readonly Dictionary<string, IAppApiAction> actions = new Dictionary<string, IAppApiAction>();

        public AppApiActionCollection(XtiPath path, ResourceAccess access, IAppApiUser user)
        {
            this.path = path;
            this.access = access;
            this.user = user;
        }

        public AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName) =>
            (AppApiAction<TModel, TResult>)actions[actionName.ToLower()];

        public IEnumerable<IAppApiAction> Actions() => actions.Values.ToArray();


        public AppApiAction<TModel, TResult> Add<TModel, TResult>
        (
            string actionName,
            Func<AppActionValidation<TModel>> createValidation,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Add(actionName, access, createValidation, createExecution, friendlyName);
        }

        public AppApiAction<TModel, TResult> Add<TModel, TResult>
        (
            string actionName,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Add(actionName, access, defaultCreateValidation<TModel>(), createExecution, friendlyName);
        }

        public AppApiAction<TModel, TResult> Add<TModel, TResult>
        (
            string actionName,
            ResourceAccess access,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = ""
        )
        {
            return Add
            (
                actionName,
                access,
                defaultCreateValidation<TModel>(),
                createExecution,
                friendlyName
            );
        }

        public AppApiAction<TModel, TResult> Add<TModel, TResult>
        (
            string actionName,
            ResourceAccess access,
            Func<AppActionValidation<TModel>> createValidation,
            Func<AppAction<TModel, TResult>> createExecution,
            string friendlyName = null
        )
        {
            if (string.IsNullOrWhiteSpace(actionName)) { throw new ArgumentException($"{nameof(actionName)} is required"); }
            var action = new AppApiAction<TModel, TResult>
            (
                path.WithAction(actionName),
                access,
                user,
                createValidation ?? defaultCreateValidation<TModel>(),
                createExecution ?? defaultCreateAction<TModel, TResult>(),
                friendlyName
            );
            actions.Add(action.ActionName.ToLower(), action);
            return action;
        }

        public static Func<AppActionValidation<TModel>> defaultCreateValidation<TModel>() =>
            () => new AppActionValidationNotRequired<TModel>();

        public static Func<AppAction<TModel, TResult>> defaultCreateAction<TModel, TResult>() =>
            () => new EmptyAppAction<TModel, TResult>();

        public AppApiAction<TModel, TResult> Add<TModel, TResult>(AppApiAction<TModel, TResult> action)
        {
            actions.Add(action.ActionName.ToLower(), action);
            return action;
        }
    }
}
