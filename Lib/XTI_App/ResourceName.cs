using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class ResourceName : TextKeyValue, IEquatable<ResourceName>
    {
        public static readonly ResourceName Unknown = new ResourceName("Unknown");

        public ResourceName(string value) : base(value)
        {
        }

        public bool Equals(ResourceName other) => _Equals(other);
    }
}
