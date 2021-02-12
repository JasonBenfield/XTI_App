using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class AppVersionType : NumericValue, IEquatable<AppVersionType>
    {
        public sealed class AppVersionTypes : NumericValues<AppVersionType>
        {
            public AppVersionTypes() : base(new AppVersionType(0, "Not Set"))
            {
                NotSet = DefaultValue;
                Major = Add(new AppVersionType(1, "Major"));
                Minor = Add(new AppVersionType(2, "Minor"));
                Patch = Add(new AppVersionType(3, "Patch"));
            }
            public AppVersionType NotSet { get; }
            public AppVersionType Major { get; }
            public AppVersionType Minor { get; }
            public AppVersionType Patch { get; }
        }

        public static readonly AppVersionTypes Values = new AppVersionTypes();

        private AppVersionType(int value, string displayText) : base(value, displayText)
        {
        }

        public bool Equals(AppVersionType other) => _Equals(other);
    }
}
