using XTI_Core;

namespace XTI_App
{
    public sealed class AppType : NumericValue
    {
        public sealed class AppTypes : NumericValues<AppType>
        {
            internal AppTypes() : base(new AppType(0, "Not Found"))
            {
                NotFound = DefaultValue;
                WebApp = Add(new AppType(10, "Web App"));
                Service = Add(new AppType(15, "Service"));
                Package = Add(new AppType(20, "Package"));
            }
            public AppType NotFound { get; }
            public AppType WebApp { get; }
            public AppType Service { get; }
            public AppType Package { get; }
        }

        public static readonly AppTypes Values = new AppTypes();

        private AppType(int value, string displayText) : base(value, displayText)
        {
        }
    }
}
