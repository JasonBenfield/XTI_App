using System;

namespace XTI_App
{
    public sealed class GeneratedKey
    {
        public string Value() => Guid.NewGuid().ToString("N");
    }
}
