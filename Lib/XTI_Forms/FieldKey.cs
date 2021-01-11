using System;

namespace XTI_Forms
{
    public sealed class FieldKey : IEquatable<FieldKey>, IEquatable<string>
    {
        public FieldKey(string prefix, string name)
        {
            Prefix = prefix;
            Name = name;
            value = string.IsNullOrWhiteSpace(Prefix) ? Name : $"{Prefix}_{Name}";
        }

        private readonly string value;

        public string Prefix { get; }
        public string Name { get; }

        public string Value() => value;

        public override bool Equals(object obj)
        {
            if(obj is FieldKey fieldKey)
            {
                return Equals(fieldKey);
            }
            if(obj is string str)
            {
                return Equals(str);
            }
            return base.Equals(obj);
        }

        public bool Equals(FieldKey other) => value == other?.value;

        public bool Equals(string other) => value == other;

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => $"{nameof(FieldKey)} {Value()}";
    }
}
