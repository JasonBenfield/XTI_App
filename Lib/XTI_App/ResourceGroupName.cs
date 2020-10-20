using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceGroupName : SemanticType<string>, IEquatable<ResourceGroupName>
    {
        public static readonly ResourceGroupName Unknown = new ResourceGroupName("");

        public ResourceGroupName(string value) : base(value?.Trim().ToLower() ?? "")
        {
        }

        public bool Equals(ResourceGroupName other) => _Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
