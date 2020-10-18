using System;

namespace XTI_Core
{
    public sealed class AppEventSeverity : NumericValue, IEquatable<AppEventSeverity>
    {
        public sealed class AppEventSeverities : NumericValues<AppEventSeverity>
        {
            public AppEventSeverities()
                : base(new AppEventSeverity(0, "Not Set"))
            {
                NotSet = DefaultValue;
                CriticalError = add(100, "Critical Error");
                AccessDenied = add(80, "Access Denied");
                AppError = add(70, "App Error");
                ValidationFailed = add(60, "Validation Failed");
            }
            private AppEventSeverity add(int value, string displayText) =>
                Add(new AppEventSeverity(value, displayText));
            public AppEventSeverity NotSet { get; }
            public AppEventSeverity CriticalError { get; }
            public AppEventSeverity AccessDenied { get; }
            public AppEventSeverity ValidationFailed { get; }
            public AppEventSeverity AppError { get; }
        }

        public static readonly AppEventSeverities Values = new AppEventSeverities();

        private AppEventSeverity(int value, string displayText)
            : base(value, displayText)
        {
        }

        public bool Equals(AppEventSeverity other) => _Equals(other);
    }
}
