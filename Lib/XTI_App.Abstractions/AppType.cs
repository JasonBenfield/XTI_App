using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class AppType : NumericValue, IEquatable<AppType>
{
    public sealed class AppTypes : NumericValues<AppType>
    {
        internal AppTypes() : base(new AppType(0, "Not Found"))
        {
            NotFound = DefaultValue;
            WebService = Add(new AppType(5, "Web Service"));
            WebApp = Add(new AppType(10, "Web App"));
            ServiceApp = Add(new AppType(15, "Service App"));
            Package = Add(new AppType(20, "Package"));
            ConsoleApp = Add(new AppType(25, "Console App"));
            WebPackage = Add(new AppType(30, "Web Package"));
        }
        public AppType NotFound { get; }
        public AppType WebService { get; }
        public AppType WebApp { get; }
        public AppType ServiceApp { get; }
        public AppType Package { get; }
        public AppType ConsoleApp { get; }
        public AppType WebPackage { get; }
    }

    public static readonly AppTypes Values = new AppTypes();

    private AppType(int value, string displayText) : base(value, displayText)
    {
    }

    public bool Equals(AppType? other) => _Equals(other);

    public bool EqualsAny(params AppType[] others) => _EqualsAny(others);
}