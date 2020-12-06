using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceGroupName : TextValue, IEquatable<ResourceGroupName>
    {
        public static readonly ResourceGroupName Unknown = new ResourceGroupName("Unknown");

        public ResourceGroupName(string value) : base(value?.Trim().ToLower() ?? "", value)
        {
        }

        public bool Equals(ResourceGroupName other) => _Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
