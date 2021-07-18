using XTI_App.Abstractions;

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

        public void SetPath(XtiPath path) => this.path = path;
    }
}
