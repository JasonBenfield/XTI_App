using System.Collections.Generic;

namespace XTI_App.Api
{
    public interface IAppApiActionCollection
    {
        IEnumerable<IAppApiAction> Actions();
        AppApiAction<TModel, TResult> Action<TModel, TResult>(string actionName);
        AppApiAction<TModel, TResult> Add<TModel, TResult>(AppApiAction<TModel, TResult> action);
    }
}