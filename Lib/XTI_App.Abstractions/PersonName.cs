using System;
using XTI_Core;

namespace XTI_App.Abstractions
{
    public sealed class PersonName : TextValue, IEquatable<PersonName>
    {
        public PersonName(string value) : base(value?.Trim() ?? "")
        {
        }

        public bool Equals(PersonName other) => _Equals(other);
    }
}
