using XTI_App.Api;

namespace XTI_WebApp.Api;

public sealed class WebAppApiActionFactory : AppApiActionFactory
{
    public WebAppApiActionFactory(AppApiGroup group) : base(group)
    {
    }

    public AppApiAction<EmptyRequest, WebViewResult> DefaultView() =>
        DefaultView(defaultCreateValidation<EmptyRequest>());

    public AppApiAction<TModel, WebViewResult> DefaultView<TModel>() =>
        DefaultView(defaultCreateValidation<TModel>());

    public AppApiAction<TModel, WebViewResult> DefaultView<TModel>
    (
        Func<AppActionValidation<TModel>> createValidation
    )
    {
        return View
        (
            "Index",
            createValidation,
            () => new DefaultViewAppAction<TModel>()
        );
    }

    public AppApiAction<TModel, WebViewResult> View<TModel>
    (
        string actionName,
        Func<AppAction<TModel, WebViewResult>> createAction
    )
    {
        return View(actionName, defaultCreateValidation<TModel>(), createAction);
    }

    public AppApiAction<TModel, WebViewResult> View<TModel>
    (
        string actionName,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebViewResult>> createAction
    )
    {
        return Action
        (
            actionName,
            createValidation,
            createAction,
            ""
        );
    }

    public AppApiAction<TModel, WebViewResult> View<TModel>
    (
        string actionName,
        ResourceAccess access,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebViewResult>> createAction
    )
    {
        return Action
        (
            actionName,
            access,
            createValidation,
            createAction,
            ""
        );
    }

    public AppApiAction<TModel, WebPartialViewResult> PartialView<TModel>
    (
        string actionName,
        Func<AppAction<TModel, WebPartialViewResult>> createAction
    )
    {
        return PartialView(actionName, defaultCreateValidation<TModel>(), createAction);
    }

    public AppApiAction<TModel, WebPartialViewResult> PartialView<TModel>
    (
        string actionName,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebPartialViewResult>> createAction
    )
    {
        return Action
        (
            actionName,
            createValidation,
            createAction,
            ""
        );
    }

    public AppApiAction<TModel, WebPartialViewResult> PartialView<TModel>
    (
        string actionName,
        ResourceAccess access,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebPartialViewResult>> createAction
    )
    {
        return Action
        (
            actionName,
            access,
            createValidation,
            createAction,
            ""
        );
    }

    public AppApiAction<TModel, WebRedirectResult> Redirect<TModel>
    (
        string actionName,
        Func<AppAction<TModel, WebRedirectResult>> createAction
    )
    {
        return Redirect(actionName, defaultCreateValidation<TModel>(), createAction);
    }

    public AppApiAction<TModel, WebRedirectResult> Redirect<TModel>
    (
        string actionName,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebRedirectResult>> createAction
    )
    {
        return Action
        (
            actionName,
            createValidation,
            createAction,
            ""
        );
    }

    public AppApiAction<TModel, WebRedirectResult> Redirect<TModel>
    (
        string actionName,
        ResourceAccess access,
        Func<AppActionValidation<TModel>> createValidation,
        Func<AppAction<TModel, WebRedirectResult>> createAction
    )
    {
        return Action
        (
            actionName,
            access,
            createValidation,
            createAction,
            ""
        );
    }
}