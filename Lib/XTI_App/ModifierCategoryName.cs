using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class ModifierCategoryName : TextValue, IEquatable<ModifierCategoryName>
    {
        public static readonly ModifierCategoryName Default = new ModifierCategoryName("Default");

        public ModifierCategoryName(string value) : base(value?.Trim().ToLower() ?? "", value)
        {
        }

        public bool Equals(ModifierCategoryName other) => _Equals(other);
    }
}
