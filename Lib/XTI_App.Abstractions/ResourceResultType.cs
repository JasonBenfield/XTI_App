using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class ResourceResultType : NumericValue, IEquatable<ResourceResultType>
    {
        public static readonly ResourceResultTypes Values = new ResourceResultTypes();

        public sealed class ResourceResultTypes : NumericValues<ResourceResultType>
        {
            public ResourceResultTypes() : base(new ResourceResultType(0, nameof(None)))
            {
                None = DefaultValue;
                View = Add(new ResourceResultType(1, nameof(View)));
                PartialView = Add(new ResourceResultType(2, nameof(PartialView)));
                Redirect = Add(new ResourceResultType(3, nameof(Redirect)));
                Json = Add(new ResourceResultType(4, nameof(Json)));
            }
            public ResourceResultType None { get; }
            public ResourceResultType View { get; }
            public ResourceResultType PartialView { get; }
            public ResourceResultType Redirect { get; }
            public ResourceResultType Json { get; }
        }

        public ResourceResultType(int value, string displayText) : base(value, displayText)
        {
        }

        public bool Equals(ResourceResultType other) => _Equals(other);
    }
}
