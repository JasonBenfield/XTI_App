using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class ModifierKey : TextKeyValue, IEquatable<ModifierKey>
    {
        public static bool operator ==(ModifierKey a, ModifierKey b) => Equals(a, b);
        public static bool operator !=(ModifierKey a, ModifierKey b) => !(a == b);

        public static readonly ModifierKey Default = new ModifierKey("");

        public static ModifierKey Generate() => new ModifierKey(new GeneratedKey().Value());

        public static ModifierKey FromValue(string value) =>
            string.IsNullOrWhiteSpace(value) ? Default : new ModifierKey(value);

        public ModifierKey(string value) : base(value)
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(ModifierKey other) => _Equals(other);
    }
}
