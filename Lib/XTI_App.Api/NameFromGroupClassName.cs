using System;

namespace XTI_App.Api
{
    public sealed class NameFromGroupClassName
    {
        public NameFromGroupClassName(string groupClassName)
        {
            const string ending = "Group";
            if (groupClassName.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
            {
                Value = groupClassName.Remove(groupClassName.Length - ending.Length);
            }
            else
            {
                Value = groupClassName;
            }
        }

        public string Value { get; }

        public override string ToString() => Value;
    }
}
