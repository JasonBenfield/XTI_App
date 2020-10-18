using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppVersionStatus : NumericValue, IEquatable<AppVersionStatus>
    {
        public sealed class AppVersionStatuses : NumericValues<AppVersionStatus>
        {
            public AppVersionStatuses() : base(new AppVersionStatus(0, "Not Set"))
            {
                New = Add(new AppVersionStatus(1, "New"));
                Publishing = Add(new AppVersionStatus(2, "Publishing"));
                Old = Add(new AppVersionStatus(3, "Old"));
                Current = Add(new AppVersionStatus(4, "Current"));
            }
            public AppVersionStatus NotSet { get; }
            public AppVersionStatus New { get; }
            public AppVersionStatus Publishing { get; }
            public AppVersionStatus Old { get; }
            public AppVersionStatus Current { get; }
        }

        public static readonly AppVersionStatuses Values = new AppVersionStatuses();

        private AppVersionStatus(int value, string displayText) : base(value, displayText)
        {
        }

        public bool Equals(AppVersionStatus other) => _Equals(other);
    }
}
