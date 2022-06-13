using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class ResourceResultType : NumericValue, IEquatable<ResourceResultType>
{
    public static readonly ResourceResultTypes Values = new ResourceResultTypes();

    public sealed class ResourceResultTypes : NumericValues<ResourceResultType>
    {
        internal ResourceResultTypes() : base(new ResourceResultType(0, nameof(None)))
        {
            None = DefaultValue;
            View = Add(new ResourceResultType(1, nameof(View)));
            PartialView = Add(new ResourceResultType(2, nameof(PartialView)));
            Redirect = Add(new ResourceResultType(3, nameof(Redirect)));
            Json = Add(new ResourceResultType(4, nameof(Json)));
            File = Add(new ResourceResultType(5, nameof(File)));
            Content = Add(new ResourceResultType(6, nameof(Content)));
            Query = Add(new ResourceResultType(7, nameof(Query)));
            QueryToExcel = Add(new ResourceResultType(8, nameof(QueryToExcel)));
        }
        public ResourceResultType None { get; }
        public ResourceResultType View { get; }
        public ResourceResultType PartialView { get; }
        public ResourceResultType Redirect { get; }
        public ResourceResultType Json { get; }
        public ResourceResultType File { get; }
        public ResourceResultType Content { get; }
        public ResourceResultType Query { get; }
        public ResourceResultType QueryToExcel { get; }
    }

    private ResourceResultType(int value, string displayText) 
        : base(value, displayText)
    {
    }

    public bool Equals(ResourceResultType? other) => _Equals(other);
}