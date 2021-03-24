using System;

namespace XTI_App.Abstractions
{
    public sealed class GeneratedKey
    {
        public string Value() => Guid.NewGuid().ToString("N");
    }
}
