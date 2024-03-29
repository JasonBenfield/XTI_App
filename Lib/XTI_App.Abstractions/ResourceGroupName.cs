﻿using XTI_Core;

namespace XTI_App.Abstractions;

public sealed class ResourceGroupName : TextKeyValue, IEquatable<ResourceGroupName>
{
    public static readonly ResourceGroupName Unknown = new ResourceGroupName("Unknown");

    public ResourceGroupName(string value) : base(value)
    {
    }

    public bool Equals(ResourceGroupName? other) => _Equals(other);
}