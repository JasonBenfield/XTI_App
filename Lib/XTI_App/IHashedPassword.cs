using System;

namespace XTI_App
{
    public interface IHashedPassword : IEquatable<string>
    {
        string Value();
    }
}
