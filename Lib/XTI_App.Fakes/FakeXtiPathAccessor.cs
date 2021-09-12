using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class FakeXtiPathAccessor : IXtiPathAccessor
    {
        private XtiPath path;

        public FakeXtiPathAccessor(XtiPath path)
        {
            this.path = path;
        }

        public XtiPath Value() => path;

        public void SetPath<TModel, TResult>(AppApiAction<TModel, TResult> action)
            => SetPath(action.Path);

        public void SetPath(XtiPath path) => this.path = path;
    }
}
